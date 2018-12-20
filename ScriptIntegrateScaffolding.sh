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

# shellcheck disable=SC2154
export PS4='${debian_chroot:+($debian_chroot)}'\
'\[\033[01;32m\]\u@\h\[\033[00m\]:\[\033[01;34m\]\w\[\033[00m\] [\D{%T}] \$ '

echo -e "\n\nStart of Script\n============\n"

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

testsBeforeStart() {
    if [[ $# -ne 1 ]]
    then
        error 5 "Should have one argument"
    fi

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
}

scaffoldingCore() {
    mv Pages "${CurrentPagesMoved}"
    isFinishHook=1
    export DOTNET_ROOT=/opt/dotnet && export PATH="$PATH:$HOME/.dotnet/tools"
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
    cd "${PagesNewEntity}"
    sed "s/Article/${NewEntity}/g" -- *
    sed "s/article/${newEntity}/g" -- *

    nvr --remote-tab _DetailsPartialView.cshtml
    nvr --remote-tab "${GeneratedEntity}/Details.cshtml"

    nvr --remote-tab _FormPartialView.cshtml
    nvr --remote-tab "${GeneratedEntity}/Edit.cshtml"

    nvr --remote-tab _ShowAllTbody.cshtml
    nvr --remote-tab ShowAll.cshtml
    nvr --remote-tab "${GeneratedEntity}/Index.cshtml"

    nvr --remote-tab "${GeneratedEntity}/CreateOrEdit.cshtml.cs"
}

main() {
    # declare: globaly if needed in a hook
    # don't forget that local has a very special meaning
    # see https://github.com/JulioJu/generator-jhipster/blob/correct-travis-build/travis/build-samples.sh
    declare -r NewEntity=${1}
    declare newEntity

    newEntity="$(tr '[:upper:]' '[:lower:]' <<< \
        "${NewEntity:0:1}")${NewEntity:1}"

    declare -r GeneratedEntity="${DIR_SCRIPT}/../Generated${NewEntity}"
    declare -r PagesGenerated="Pages/Generated"
    declare -r PagesNewEntity="Pages/${NewEntity}"
    declare -r CurrentPagesMoved="/tmp/Pages"

    testsBeforeStart "${@}"

    set -x
    scaffoldingCore
    set +x
}

declare DIR_SCRIPT
DIR_SCRIPT="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd -P)"
cd "${DIR_SCRIPT}"

declare -r URED="\\033[4;31m"
declare -r NC="\\033[0m"


declare -i isFinishHook=0

main "${@}"
