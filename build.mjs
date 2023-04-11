#!/usr/bin/env zx

import { existsSync, mkdirSync, rmSync } from "fs";

if (existsSync(".build")) {
  rmSync(".build", { recursive: true });
}

mkdirSync(".build");
mkdirSync(".build/SgBlogApi.CreatePost");
mkdirSync(".build/SgBlogApi.GetPost");
mkdirSync(".build/SgBlogApi.UpdatePost");

const name = "sg-blog-api";

const existing = await $`docker ps -aqf "name=${name}"`;
if (existing.stdout) {
  await $`docker rm ${name}`;
}

await $`docker build -t ${name} -f Dockerfile .`;

const run = await $`docker run -d --name ${name} ${name}:latest`;
const containerId = run.stdout.trim();

await $`docker cp ${containerId}:/source/src/SgBlogApi.CreatePost/bin/Release/net7.0/linux-x64/native/bootstrap .build/SgBlogApi.CreatePost/bootstrap`;
await $`docker cp ${containerId}:/source/src/SgBlogApi.GetPost/bin/Release/net7.0/linux-x64/native/bootstrap .build/SgBlogApi.GetPost/bootstrap`;
await $`docker cp ${containerId}:/source/src/SgBlogApi.UpdatePost/bin/Release/net7.0/linux-x64/native/bootstrap .build/SgBlogApi.UpdatePost/bootstrap`;
