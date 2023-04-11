#!/usr/bin/env zx

import { existsSync, mkdirSync, rmSync } from "fs";

if (existsSync(".build")) {
  rmSync(".build", { recursive: true });
}

mkdirSync(".build");
mkdirSync(".build/SgBlogApi.CreatePost");
mkdirSync(".build/SgBlogApi.GetPost");
mkdirSync(".build/SgBlogApi.UpdatePost");
mkdirSync(".build/SgBlogApi.DeletePost");
mkdirSync(".build/SgBlogApi.ListPost");

const name = "sg-blog-api";

const existing = await $`docker ps -aqf "name=${name}"`;
if (existing.stdout) {
  await $`docker rm ${name}`;
}

await $`docker build -t ${name} -f Dockerfile .`;

const run = await $`docker run -d --name ${name} ${name}:latest`;
const containerId = run.stdout.trim();

await $`docker cp ${containerId}:/source/src/SgBlogApi.CreatePost/bin/Release/net7.0/linux-x64/native/. .build/SgBlogApi.CreatePost`;
await $`docker cp ${containerId}:/source/src/SgBlogApi.GetPost/bin/Release/net7.0/linux-x64/native/. .build/SgBlogApi.GetPost`;
await $`docker cp ${containerId}:/source/src/SgBlogApi.UpdatePost/bin/Release/net7.0/linux-x64/native/. .build/SgBlogApi.UpdatePost`;
await $`docker cp ${containerId}:/source/src/SgBlogApi.DeletePost/bin/Release/net7.0/linux-x64/native/. .build/SgBlogApi.DeletePost`;
await $`docker cp ${containerId}:/source/src/SgBlogApi.ListPost/bin/Release/net7.0/linux-x64/native/. .build/SgBlogApi.ListPost`;

await $`mv ./.build/SgBlogApi.CreatePost/SgBlogApi.CreatePost ./.build/SgBlogApi.CreatePost/bootstrap`;
await $`mv ./.build/SgBlogApi.CreatePost/SgBlogApi.CreatePost.dbg ./.build/SgBlogApi.CreatePost/bootstrap.dbg`;

await $`mv ./.build/SgBlogApi.GetPost/SgBlogApi.GetPost ./.build/SgBlogApi.GetPost/bootstrap`;
await $`mv ./.build/SgBlogApi.GetPost/SgBlogApi.GetPost.dbg ./.build/SgBlogApi.GetPost/bootstrap.dbg`;

await $`mv ./.build/SgBlogApi.UpdatePost/SgBlogApi.UpdatePost ./.build/SgBlogApi.UpdatePost/bootstrap`;
await $`mv ./.build/SgBlogApi.UpdatePost/SgBlogApi.UpdatePost.dbg ./.build/SgBlogApi.UpdatePost/bootstrap.dbg`;

await $`mv ./.build/SgBlogApi.DeletePost/SgBlogApi.DeletePost ./.build/SgBlogApi.DeletePost/bootstrap`;
await $`mv ./.build/SgBlogApi.DeletePost/SgBlogApi.DeletePost.dbg ./.build/SgBlogApi.DeletePost/bootstrap.dbg`;

await $`mv ./.build/SgBlogApi.ListPost/SgBlogApi.ListPost ./.build/SgBlogApi.ListPost/bootstrap`;
await $`mv ./.build/SgBlogApi.ListPost/SgBlogApi.ListPost.dbg ./.build/SgBlogApi.ListPost/bootstrap.dbg`;
