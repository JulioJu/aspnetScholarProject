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

usage() {
    error 5 "${NC}Should have either one or two arguments." \
        "\n\t* First one: name of the entity to generate." \
        "\n\t* Seconde one --is-generation-again"
}

testCommandLine(){
    if [[ $# -ne 1 ]] && [[ $# -ne 2 ]]
    then
        usage
    fi
    if [[ $# -eq 2 ]]  && [[ ! ${2} = "--is-generation-again" ]]
    then
        usage
    fi
}

testsBeforeStart() {
    if [[ ${isGenerationAgain} -eq 0 ]] && [[ -e "${PagesNewEntity}" ]]
    then
        error 6 "'${PagesNewEntity}' already generated. " \
        " Specify a second argument '--is-generation-again'"
    fi
    if [[ ${isGenerationAgain} -eq 1 ]] && [[ ! -e "${PagesNewEntity}" ]]
    then
        error 6 "'Page/${PagesNewEntity}' doesn't exist." \
            "Remove argument '--is-generation-again'"
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

userInputSimple() {
    read -r -t 1 -n 10000 _ || echo ""
    echo -e "Press 'ENTER' to continue"
    read -r -p " "
}

userInputCorrectAllFiles() {
    read -r -t 1 -n 10000 _ || echo ""
    echo -e "Press 'ENTER' to continue" \
        "${URED}WHEN ALL FILE WILL BE CORRECTED${NC}"
    read -r -p " "
}

retrieveMdpAndDatabaseName() {
    # if [[ $# -eq 1 ]]
    # then
    #     local -ri isThreeArguments=0
    # else
    #     local -ri isThreeArguments=1
    # fi

    # if [[ ${isThreeArguments} -eq 1 ]]
    # then
    #     local -r PathOfTheMdpFile=${2}
    #     DatabaseName=${3}
    #
    #     # Note: in LocalDB there isn't password
    #     # Note2: the password should be removed by `git checkout' or `git stash'
    #     #   because the following command should be executed:
    #     #   `git update-index --assume-unchanged ./appsettings.json'
    #     if [[ ! -f "${PathOfTheMdpFile}" ]] ||
    #         [[ "$(cut -d '' -f 2 < \
    #                 <(file -F '' --mime-type "${PathOfTheMdpFile}"))" \
    #             != " text/plain" ]]
    #     then
    #         error 11 "The file '${PathOfTheMdpFile}'" \
    #             " does not exist or is not a 'text/plain' file."
    #     else
    #         # `echo -n "aa" > file && wc -l file` ==> 0
    #         local -i isLastLineNotCarriageReturn=0
    #         if [[ -n $(tail -c 1 "${PathOfTheMdpFile}") ]]
    #         then
    #             isLastLineNotCarriageReturn=1
    #         fi
    #         local -i fileMdpNumberOfLines
    #         fileMdpNumberOfLines=$(($(wc -l < \
    #             "${PathOfTheMdpFile}")+isLastLineNotCarriageReturn))
    #         if [[ ${fileMdpNumberOfLines} -ne 1 ]]
    #         then
    #             error 13 "The file '${PathOfTheMdpFile}'" \
    #                 "does not contain only one line"
    #         fi
    #
    #         local -i fileMdpNumberOfChars
    #         fileMdpNumberOfChars=$(wc -c < <(tr -d '\n' < \
    #             <(tr -d '\r\n' < "${PathOfTheMdpFile}")))
    #         if [[ ${fileMdpNumberOfChars} -lt 8 ]] \
    #             || [[ ${fileMdpNumberOfChars} -gt 30 ]]
    #         then
    #             error 12 "The file '${PathOfTheMdpFile}'" \
    #                 "does not contain between 8 and 30 chars."
    #         fi
    #
    #         Mdp
    #         if [[ -z $(tail -c 1 "${PathOfTheMdpFile}") ]]
    #         then
    #             Mdp=$(head -c -1 "${PathOfTheMdpFile}")
    #         else
    #             Mdp=$(cat "${PathOfTheMdpFile}")
    #         fi
    #     fi
    # else
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
        echo "$DatabaseName"
    # fi
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

resetFolderPosition() {
    mv "${CurrentPagesMoved}"/* ./Pages
    rmdir "${CurrentPagesMoved}"
}

scaffoldingCore() {
    mv Pages "${CurrentPagesMoved}"
    if ! dotnet aspnet-codegenerator razorpage -m "${NewEntity}" -dc \
        AppDbContext \
        -udl -outDir "${PagesGenerated}" --referenceScriptLibraries
    then
        resetFolderPosition
        error 9 "'${PagesGenerated}' not generated"
    fi
    if [[ ! -d "${PagesGenerated}" ]]
    then
        resetFolderPosition
        error 9 "'${PagesGenerated}' not generated"
    fi

    mv "${PagesGenerated}" "${GeneratedEntity}"

    resetFolderPosition

    if [[ ${isGenerationAgain} -eq 0 ]]
    then
        mkdir "${PagesNewEntity}"
        cp Pages/Article/* "${PagesNewEntity}"
    fi

    if [[ ${isGenerationAgain} -eq 0 ]]
    then
        sed -i "1,2d
            s/public DbSet<.*${NewEntity}> ${NewEntity} { get; set; }/internal DbSet<${NewEntity}> ${NewEntity}s { get; set; }/
            //! s/$//" ./Entities/AppDbContext.cs

    fi

    sed -i "s/${NewEntity}\.//g
        s/\.${NewEntity}\[/\.AbstractEntities[/g" \
            "${GeneratedEntity}/Details.cshtml" \
            "${GeneratedEntity}/Edit.cshtml" \
            "${GeneratedEntity}/Index.cshtml"

    pushd "${PagesNewEntity}"

    if [[ ${isGenerationAgain} -eq 0 ]]
    then
        sed -i "s/Article/${NewEntity}/g" -- *
        sed -i "s/article/${newEntity}/g" -- *
    fi

    set +x

    echo -e "\n\nFile to edit will be open in new NeoVim tabs" \
            "thanks Neovim Remote" \
            "(script should be launched in a Neovim Terminal)." \
        "\n* In the last tab (10th), move the line added with" \
        "\`Dbset<${NewEntity}>' at the beginning of the file." \
        "\n* In the 9th tab, correct errors"  \
            "DO NOT FORGET to correct OnGetAsync() function" \
            "(no warning displayed)" \
        "\n* In the 8th tab, correct errors"  \
        "\n* Copy content of the 7th tab in the 6th and 5th tab," \
            "CORRECT INDENTATION, " \
            "DO NOT FORGET TO REMOVE UPDATED_DATE AND CREATED_DATE FIELDS." \
        "\n* Copy content of the 4th tab in the 3th tab," \
            "CORRECT INDENTATION, " \
            "DO NOT FORGET TO REMOVE UPDATED_DATE AND CREATED_DATE FIELDS." \
        "\n* Copy content of the 2th in the 1th tab," \
            "CORRECT INDENTATION, " \
            "DO NOT FORGET TO REMOVE UPDATED_DATE AND CREATED_DATE FIELDS."

    userInputSimple

    rm -f _TableFormDetailsThead.cshtml _TableFormDetailsTbody.cshtml

    nvr --remote-tab _DetailsPartialView.cshtml
    nvr --remote-tab "${GeneratedEntity}/Details.cshtml"

    nvr --remote-tab _FormPartialView.cshtml
    nvr --remote-tab "${GeneratedEntity}/Edit.cshtml"

    nvr --remote-tab _ShowAllTbody.cshtml
    nvr --remote-tab ShowAll.cshtml
    nvr --remote-tab "${GeneratedEntity}/Index.cshtml"

    nvr --remote-tab Crud.cshtml.cs

    nvr --remote-tab ShowAll.cshtml.cs

    popd

    nvr --remote-tab ./Entities/AppDbContext.cs

    userInputCorrectAllFiles

    set -x
    sqlcmd -S localhost -U SA -P "${Mdp}" \
        -Q "drop database ${DatabaseName}"

    rm -Rf Migrations
    while ! dotnet ef migrations add InitialCreate
    do
        set +x
        echo -e "${URED}Error during build. You havn't correct all errors${NC}"
        userInputCorrectAllFiles
    done
    dotnet ef database update
    set +x
}

main() {
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

    local -i isGenerationAgain=0
    if [[ $# -eq 2 ]]  && [[ ${2} = "--is-generation-again" ]]
    then
        isGenerationAgain=1
    fi
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
export PS4='

${debian_chroot:+($debian_chroot)}'\
'\[\033[01;32m\]\u@\h\[\033[00m\]:\[\033[01;34m\]\w\[\033[00m\] [\D{%T}] \$ '
declare -g -r PS4Light="\n\n\\033[1;32m""$USER@""$HOSTNAME""\\033[0m"": "

declare -g DIR_SCRIPT
DIR_SCRIPT="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd -P)"
cd "${DIR_SCRIPT}"

declare -g -r URED="\\033[4;31m"
declare -g -r NC="\\033[0m"

testCommandLine "${@}"
main "${@}"
