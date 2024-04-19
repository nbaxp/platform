#!/bin/bash

APP="Wta"

set -e
DIR=$(
  cd $(dirname $0)
  pwd
)
cd ${DIR}

pid=$(ps -ef | grep ${DIR}/${APP} | grep -v grep | awk '{print $2}' )
pid=${pid:=0}

if [ $pid -gt 0 ]; then
  kill -9 $pid
fi
