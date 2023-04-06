#!/usr/bin/env node
import "source-map-support/register";
import * as cdk from "aws-cdk-lib";
import { AppStack } from "../lib/app-stack";
import { get } from "env-var";
import * as dotenv from "dotenv";

dotenv.config();

const CDK_DEFAULT_ACCOUNT = get("CDK_DEFAULT_ACCOUNT").required().asString();
const CDK_DEFAULT_REGION = get("CDK_DEFAULT_REGION").required().asString();
const SERVICE = get("SERVICE").required().asString();
const STAGE = get("STAGE").required().asString();

const appStackName = `${SERVICE}-${STAGE}-app`;
const app = new cdk.App();

new AppStack(app, appStackName, {
  description: `${SERVICE} ${STAGE} application stack`,
  service: SERVICE,
  stage: STAGE,
  env: {
    account: CDK_DEFAULT_ACCOUNT,
    region: CDK_DEFAULT_REGION,
  },
});
