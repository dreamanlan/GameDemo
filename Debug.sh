#!/bin/sh

WORK_DIR=$(cd `dirname $0`; pwd)
cd ${WORK_DIR}

mono BatchCommand.exe ./Tools/Batch/build.dsl Debug
