#!/bin/bash

docker pull rubeha/icode_api
docker pull rubeha/icode_web
docker pull rubeha/code_executor

docker-compose down
docker-compose up -d