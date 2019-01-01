#!/bin/bash
# -*- coding: UTF8 -*-

#===============================================================================
#
#         USAGE: See README.md
#
#   DESCRIPTION:
#
#       OPTIONS: ---
#  REQUIREMENTS: ---
#          BUGS: ---
#         NOTES: ---
#        AUTHOR: http://github.com/JulioJu
#  ORGANIZATION:
#       CREATED: 12/13/2018 17:26
#      REVISION:  ---
#===============================================================================

trap 'kill' HUP
trap 'kill' INT
trap 'kill' QUIT
trap 'finishError "$LINENO"' ERR
trap 'finish' EXIT
# Cannot be trapped
# trap 'kill' KILL
trap 'kill' TERM

# set -euET
set -euET
# Note: `set +E' doesn't work
# set -x

close() {
    # "_" is the throwaway variable
    # sleep 2
    read -r -t 1 -n 10000 _ || echo ""
    read -r -p "Press 'ENTER' to close"
}

kill() {
    set +x
    1>&2 echo -e "\\n\\n\\n""${URED}""Killed by user""${NC}""\\n\\n"
    exit 130
}

finishError() {
    set +x
    1>&2 echo "In the script, error on line: '$1'"
    exit 2
    # Otherwise, all script it's executed
}

finish() {
    returnCode=$?

    if [[ ${isFinishHook} -eq 1 ]] ; then
        mv "${CurrentPagesMoved}" ./Pages
    fi

    set +x
    if [[ "${returnCode}" -gt 0 ]] ; then
        1>&2 echo -e "\\n\\n\\n${URED}ERROR" \
            "with code '${returnCode}'${NC}\\n\\n"
        close
    else
        echo -e "\\n\\n\\n""${URED}""SUCCESS""${NC}""\\n\\n"
        close
    fi
    echo -e "\n\n\n"
}

error() {
    set +x
    1>&2 echo -e "\\n\\n\\n${URED}ERROR:" "${@:2}" "${NC}\\n\\n"
    exit "${@:1:1}"
}

testCommandLine(){
    if [[ $# -ne 1 ]] && [[ $# -ne 3 ]]
    then
        error 5 "${NC}Should have either one or three arguments." \
            "\n\t* First one: name of the entity to generate." \
            "\n\t* Seconde one (optional): path of file where mdp of the" \
                "Database is stored" \
            "\n\t* Third one (optional): name of the Database" \
                "(it will be droped, then generated again).${NC}"
    fi
}

testsBeforeStart() {
    if [[ -e "${PagesNewEntity}" ]]
    then
        error 6 "'${PagesNewEntity}' already generated."
    fi

    if ! git diff --exit-code > /dev/null
    then
        error 7 "There are unstaged changes"
    fi

    if [[ -e "${CurrentPagesMoved}" ]] \
        || [[ -e "${PagesGenerated}" ]] \
        || [[ -e "${GeneratedEntity}" ]]
    then
        error 8 "Please delete '${CurrentPagesMoved}' and / or" \
            "'${PagesGenerated}' and / or " \
            "'${GeneratedEntity}'"
    fi

    if [[ -z ${NVIM_LISTEN_ADDRESS+x} ]]
    then
        error 9 "Terminal not launched in NeoVim"
    else
        echo "Cool, Terminal launched in NeoVim"
    fi

    if ! command -V nvr
    then
        error 10 "Please install \`NeoVim Remote'"
    fi

    echo -e "${PS4Light}systemctl is-active mssql-server.service"
    if ! systemctl is-active mssql-server.service
    then
        error 14 "Please activate the service \`mssql-server'"
    fi
    echo

    if [[ -z ${DOTNET_ROOT+x} ]] \
        || ! dotnet ef > /dev/null
    then
        error 15 "* Please \`$ export DOTNET_ROOT'. Probably the command is:" \
            "\`$ DOTNET_ROOT=/opt/dotnet'" \
            "\n* Maybe a folder \`.dotnet/tools' is not in the PATH." \
            "Probably the command is: " \
            "\`$ export PATH=\"\$PATH:$HOME/.dotnet/tools\"''"
    fi


}

retrieveMdpAndDatabaseName() {
    if [[ $# -eq 1 ]]
    then
        local -ri isThreeArguments=0
    else
        local -ri isThreeArguments=1
    fi

    if [[ ${isThreeArguments} -eq 1 ]]
    then
        local -r PathOfTheMdpFile=${2}
        DatabaseName=${3}

        # Note: in LocalDB there isn't password
        # Note2: the password should be removed by `git checkout' or `git stash'
        #   because the following command should be executed:
        #   `git update-index --assume-unchanged ./appsettings.json'
        if [[ ! -f "${PathOfTheMdpFile}" ]] ||
            [[ "$(cut -d '' -f 2 < \
                    <(file -F '' --mime-type "${PathOfTheMdpFile}"))" \
                != " text/plain" ]]
        then
            error 11 "The file '${PathOfTheMdpFile}'" \
                " does not exist or is not a 'text/plain' file."
        else
            # `echo -n "aa" > file && wc -l file` ==> 0
            local -i isLastLineNotCarriageReturn=0
            if [[ -n $(tail -c 1 "${PathOfTheMdpFile}") ]]
            then
                isLastLineNotCarriageReturn=1
            fi
            local -i fileMdpNumberOfLines
            fileMdpNumberOfLines=$(($(wc -l < \
                "${PathOfTheMdpFile}")+isLastLineNotCarriageReturn))
            if [[ ${fileMdpNumberOfLines} -ne 1 ]]
            then
                error 13 "The file '${PathOfTheMdpFile}'" \
                    "does not contain only one line"
            fi

            local -i fileMdpNumberOfChars
            fileMdpNumberOfChars=$(wc -c < <(tr -d '\n' < \
                <(tr -d '\r\n' < "${PathOfTheMdpFile}")))
            if [[ ${fileMdpNumberOfChars} -lt 8 ]] \
                || [[ ${fileMdpNumberOfChars} -gt 30 ]]
            then
                error 12 "The file '${PathOfTheMdpFile}'" \
                    "does not contain between 8 and 30 chars."
            fi

            Mdp
            if [[ -z $(tail -c 1 "${PathOfTheMdpFile}") ]]
            then
                Mdp=$(head -c -1 "${PathOfTheMdpFile}")
            else
                Mdp=$(cat "${PathOfTheMdpFile}")
            fi
        fi
    else
        if [[ ! -f ./appsettings.json ]]
        then
            echo 19 "'./appsettings.json' does not exist." \
                "Please use a file './appsettings.json'"
        fi
        Mdp=$(grep --color=never -o 'password=.*"\s*$' ./appsettings.json \
            | sed 's/"\s*$//
                    s/;.*//
                    s/^password=//
                    '
            )
        DatabaseName=$(grep --color=never -o 'Database=.*"\s*$' \
                ./appsettings.json \
            | sed 's/"\s*$//
                    s/;.*//
                    s/^Database=//
                    '
            )
        echo $DatabaseName
    fi
    if [[ -z ${Mdp+x} ]]
    then
        error 20 "No Password found. Maybe you try to use LocalDB" \
            "(not compatabile with Linux)."
    fi
    if [[ ${Mdp} =~ XXXX ]]
    then
        error 21 "'${Mdp}' in ./appsettings.json" \
            "is probably not a correct Password."
    fi
    if [[ -z ${DatabaseName+x} ]]
    then
        error 22 "'${Mdp}' no Database name found."
    fi
}

scaffoldingCore() {
    mv Pages "${CurrentPagesMoved}"
    isFinishHook=1
    dotnet aspnet-codegenerator razorpage -m "${NewEntity}" -dc AppDbContext \
        -udl -outDir "${PagesGenerated}" --referenceScriptLibraries
    mv "${CurrentPagesMoved}"/* ./Pages
    rmdir "${CurrentPagesMoved}"
    isFinishHook=0
    if [[ ! -d "${PagesGenerated}" ]]
    then
        error 9 "'${PagesGenerated}' not generated"
    fi

    mv "${PagesGenerated}" "${GeneratedEntity}"

    mkdir "${PagesNewEntity}"
    cp Pages/Article/* "${PagesNewEntity}"

    sed -i "1,2d
        s/public DbSet<.*${NewEntity}> ${NewEntity} { get; set; }/internal DbSet<${NewEntity}> ${NewEntity}s { get; set; }/
        //! s/$//" ./Entities/AppDbContext.cs

    sed -i "s/${NewEntity}\.//
        s/\.${NewEntity}\[/\.AbstractEntities[/" \
            "${GeneratedEntity}/Details.cshtml" \
            "${GeneratedEntity}/Edit.cshtml" \
            "${GeneratedEntity}/Index.cshtml"

    pushd "${PagesNewEntity}"

    sed -i "s/Article/${NewEntity}/g" -- *
    sed -i "s/article/${newEntity}/g" -- *

    set +x

    echo -e "\n\nFile to edit will be open in new NeoVim tabs" \
            "thanks Neovim Remote" \
            "(script should be launched in a Neovim Terminal)." \
        "\n* In the last tab (ninth), move the line added with" \
        "\`Dbset<${NewEntity}>' at the beginning of the file." \
        "\n* In the height tab, correct errors"  \
            "DO NOT FORGET to correct OnGetAsync() function" \
            "(no warning displayed)"
        "\n* Copy content of the seventh tab in the fifth and sixth tab," \
            "CORRECT INDENTATION, " \
            "DO NOT FORGET TO REMOVE UPDATED_DATE AND CREATED_DATE FIELDS." \
        "\n* Copy content of the fourth tab in the third tab," \
            "CORRECT INDENTATION, " \
            "DO NOT FORGET TO REMOVE UPDATED_DATE AND CREATED_DATE FIELDS." \
        "\n* Copy content of the second tab in the first tab," \
            "CORRECT INDENTATION, " \
            "DO NOT FORGET TO REMOVE UPDATED_DATE AND CREATED_DATE FIELDS."

    read -r -t 1 -n 10000 _ || echo ""
    echo -e "Press 'ENTER' to continue"
    read -r -p " "

    nvr --remote-tab _DetailsPartialView.cshtml
    nvr --remote-tab "${GeneratedEntity}/Details.cshtml"

    nvr --remote-tab _FormPartialView.cshtml
    nvr --remote-tab "${GeneratedEntity}/Edit.cshtml"

    nvr --remote-tab _ShowAllTbody.cshtml
    nvr --remote-tab ShowAll.cshtml
    nvr --remote-tab "${GeneratedEntity}/Index.cshtml"

    nvr --remote-tab Crud.cshtml.cs

    popd

    nvr --remote-tab ./Entities/AppDbContext.cs

    read -r -t 1 -n 10000 _ || echo ""
    echo -e "Press 'ENTER' to continue" \
        "${URED}WHEN ALL FILE WILL BE CORRECTED${NC}"
    read -r -p " "

    set -x
    sqlcmd -S localhost -U SA -P "${Mdp}" \
        -Q "drop database ${DatabaseName}"

    rm -Rf Migrations
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    set +x
}

main() {
    testCommandLine "${@}"
    # local: globaly if needed in a hook
    # don't forget that local has a very special meaning
    # see https://github.com/JulioJu/generator-jhipster/blob/correct-travis-build/travis/build-samples.sh
    local -r NewEntity=${1}
    local newEntity
    newEntity="$(tr '[:upper:]' '[:lower:]' <<< \
        "${NewEntity:0:1}")${NewEntity:1}"

    local -r GeneratedEntity="${DIR_SCRIPT}/../Generated${NewEntity}"
    local -r PagesGenerated="Pages/Generated"
    local -r PagesNewEntity="Pages/${NewEntity}"
    local -r CurrentPagesMoved="/tmp/Pages"

    testsBeforeStart "${@}"

    local Mdp
    local DatabaseName
    retrieveMdpAndDatabaseName "${@}"

    set -x
    scaffoldingCore
    set +x
}

echo -e "\n\nStart of Script\n============\n"

# shellcheck disable=SC2154
export PS4='${debian_chroot:+($debian_chroot)}'\
'\[\033[01;32m\]\u@\h\[\033[00m\]:\[\033[01;34m\]\w\[\033[00m\] [\D{%T}] \$ '
declare -g -r PS4Light="\\033[1;32m""$USER@""$HOSTNAME""\\033[0m"": "

declare -g DIR_SCRIPT
DIR_SCRIPT="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd -P)"
cd "${DIR_SCRIPT}"

declare -g -r URED="\\033[4;31m"
declare -g -r NC="\\033[0m"


declare -g -i isFinishHook=0

main "${@}"
