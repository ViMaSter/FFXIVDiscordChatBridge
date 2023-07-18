# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="0.1.1"></a>
## [0.1.1](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v0.1.1) (2023-7-18)

### Bug Fixes

* **Consumer/FFXIV:** Skip message early to prevent exception when parsing irrelevant messages ([d5e649f](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/d5e649feb770d1ccf1f88a6ee2941cddd421a1e2))
* **README:** Sets proper Markdown line-breaks for paragraphs ([942a9a7](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/942a9a744c2f31fc74d7811299438895035bf902))

<a name="0.1.0"></a>
## [0.1.0](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v0.1.0) (2023-7-16)

### Features

* **Consumer/FFXIV:** Add --ffxivChannelCode to dynamically configure the consumed channel ([29725fc](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/29725fc029a4bed4fbc0afb097b32786e826f2ad))
* **Consumer/FFXIV:** Handles byte streams to allow for exclusion of hosting character (to prevent endless loops), irrelevant channel messages and parse FFXIV-specifics ([07a2b8d](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/07a2b8d506267f1282547015b86c0019db836bde))
* **Consumer/FFXIV, Producer/Discord:** Forward all messages from FFXIV to Discord ([7beef62](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/7beef62bb8cdab7a750b7996d38e479b55904574))

### Other

* Adds README.MD ([fb0dbd7](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/fb0dbd744ec6b96bcea99a8d833b62042d0d2bea))
* Limits Push builds to `main` branch ([4c8deb2](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/4c8deb256782b8ad872986cddcef605ef639a017))
* Queues automerge after completed run of "Build, Release, Publish" pipeline ([935f398](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/935f39896c452c619414f8c3b1a78c450170c4eb))
* Typo ([2f32966](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/2f3296675709bde36c0affab3ca7e30f97009f67))
* **deps:** Bump coverlet.collector in /FFXIVDiscordChatBridge.Test ([8246cdf](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/8246cdf170beed44a3a108bcae678a276b940ea3))
* **deps:** Bump Microsoft.NET.Test.Sdk in /FFXIVDiscordChatBridge.Test ([2bd6259](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/2bd6259418387ca9c1f02a0b350c5a4b31f63bd6))
* **deps:** Bump NUnit.Analyzers in /FFXIVDiscordChatBridge.Test ([a831b2a](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/a831b2ad84252d83fe7ffae788172e230266c211))
* **deps:** Bump NUnit3TestAdapter in /FFXIVDiscordChatBridge.Test ([79a0bda](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/79a0bdacddf4159440e75188d876dbec63bec2d5))
* **README:** Updates usage guide ([61f2020](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/61f2020d292bd2c2275eb34a502126a9bcbe47a1))

<a name="0.0.0"></a>
## [0.0.0](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v0.0.0) (2023-7-16)

### Features

* **GitHub Actions:** Adds workflows for automated release and dependency management ([da836d5](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/da836d54672b8ba6a8180858e1c19c04c48e692c))
* **producer/ffxiv:** Initial commit to send "hello" to FFXIV DX11 ([2b917d8](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/2b917d8a9acb30f505c5ca07b46e910b31434380))

### Other

* Initial commit ([b23f038](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/b23f038e695a386e491ed58da2b09f227acc52d1))

