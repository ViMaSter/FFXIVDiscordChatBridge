# Change Log

All notable changes to this project will be documented in this file. See [versionize](https://github.com/versionize/versionize) for commit guidelines.

<a name="2.4.1"></a>
## [2.4.1](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v2.4.1) (2023-8-14)

### Bug Fixes

* Emote reactions sent to FFXIV are wrapped with : for server-specific emoji ([61d2e14](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/61d2e14def4a358097a80b3036815ac5d773f427))

### Other

* **deps:** Bump Discord.Net in /FFXIVDiscordChatBridge ([541867e](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/541867ed012bb14c1bd9af58fc07c9a130d53554))
* **deps:** Bump Microsoft.Extensions.Http.Polly ([89169dc](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/89169dc7ac2568894c2fd20e4e3706ba2c915558))
* **deps:** Bump NLog from 5.2.2 to 5.2.3 in /FFXIVDiscordChatBridge ([a3c7609](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/a3c7609a1af6637ef3a5041352ba2f5832a849ad))
* **deps:** Bump NLog.Extensions.Logging in /FFXIVDiscordChatBridge ([660c5bc](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/660c5bc4e08ec645119f0e7954d58a37a50b7117))

<a name="2.4.0"></a>
## [2.4.0](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v2.4.0) (2023-8-6)

### Features

* Forwards Discord reactions to FFXIV ([ec34056](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/ec3405642464209945279d2fcf7f35d7d5772c7e))

### Bug Fixes

* Handles non-server-specific emotes ([8cfd0c8](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/8cfd0c889620d57c1701d4e43e0839f2ac37037d))

<a name="2.3.0"></a>
## [2.3.0](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v2.3.0) (2023-8-6)

### Features

* Decorates Discord replies inside FFXIV ([4f621ef](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/4f621ef6f481137c177e1eaa413bfb44a6387d70))

<a name="2.2.1"></a>
## [2.2.1](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v2.2.1) (2023-8-6)

### Bug Fixes

* Exit app on fatal errors ([252931b](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/252931b335e07cd7e2c738ab43d0cbc9060c0472))
* Uses fallback Discord Avatar URL for FFXIV players without lodestone avatar ([afcb4e3](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/afcb4e3a404cf1b0950abf6aa32a447c0e7a291b))

<a name="2.2.0"></a>
## [2.2.0](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v2.2.0) (2023-7-31)

### Features

* Discord Consumer handles edited messages by `Send()`ing entire message with `(edit)` prefix ([62b79bc](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/62b79bc6c03643a41db60a38e3324abf5de4aea6))

<a name="2.1.1"></a>
## [2.1.1](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v2.1.1) (2023-7-30)

### Bug Fixes

* Correctly maps FFXIV and Discord usernames inside Discord ([e9d2d3b](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/e9d2d3b9bd45b2236299da7e5e2a993ce7ffeb40))

<a name="2.1.0"></a>
## [2.1.0](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v2.1.0) (2023-7-30)

### Features

* Replace in-game <flag> location markers with readable output on Discord ([47ef716](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/47ef716928f1bf93ceeae3b2df38c435738fb486))

### Other

* Added helper console app to convert log-format binary into raw binary files ([73517ee](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/73517eee1d61b614cf10e3f1e5fda0e4dc7e0c6f))
* Adds macOS attributes file ([b039f5f](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/b039f5f58f6f8bbce9bacf59c44031f7769e7583))
* binary files for flag locations ([ad9ea33](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/ad9ea33740162cb87b6ccbe232af8ab8b017a372))
* Replaced ambiguous `item` prefix inside PartyFinderLinkReplacer and LocationLinkReplacer with proper prefix ([0ac14e2](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/0ac14e278be90e29a839da44f1d7ff4ee8493fc2))
* **BinaryFromLogGenerator:** Reduces complexity ([2319d50](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/2319d5054aa57389d3e170e604533467746670ed))
* **location.binary:** Replaces empty file with proper data for test fixture ([0026440](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/00264407d7852a6f04571b7adf9abcd9a32cf009))

<a name="2.0.0"></a>
## [2.0.0](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v2.0.0) (2023-7-30)

### Features

* Discord Producer represents in-game characters as different Discord users including avatars by using Discord Webhooks to send messages ([cd9c8b4](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/cd9c8b47b08c5fcafbd2abc74b4f3e3cc9e6c1d2))

### Breaking Changes

* Discord Producer represents in-game characters as different Discord users including avatars by using Discord Webhooks to send messages ([cd9c8b4](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/cd9c8b47b08c5fcafbd2abc74b4f3e3cc9e6c1d2))

### Other

* Adds screenshot ([185376f](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/185376f7d8934eefc11d7fb8bb164bc9fb8311ce))

<a name="1.4.0"></a>
## [1.4.0](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v1.4.0) (2023-7-23)

### Features

* Handles sending server-specific Discord emotes without the use of IDs ([21c804a](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/21c804a4150fa052c620a7ccee0541d41e81842b))
* Translate global Discord emoji into text ([79366a6](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/79366a66572b4c8f739c7f6b16bea2f0ed376b26))

<a name="1.3.0"></a>
## [1.3.0](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v1.3.0) (2023-7-23)

### Features

* Allows linking Discord and FFXIV usernames ([e6048fb](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/e6048fb7ced42dcf4ad843dadef1c04153bddefc))
* Persist username mappings to file inside working directory ([e8bc300](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/e8bc30001cdc66a7971a660005d33bb26c77acb4))
* Use Discord display names if available ([7dd0596](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/7dd05969e20a926ea4d667ae6603b6c7c2b65bdf))

### Other

* Circumvents /tell limitation by announcing username/character links in the monitored FF chat ([fe8be4a](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/fe8be4a15b5a23af4efa3ce047817cadb23aafee))
* Handle case-insensitive confirmations ([cfcca93](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/cfcca93082b2583d20b5b774914088c29eacc277))
* Increases coverage ([fa97ede](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/fa97ede81be0470a403f283f8b6b19d6ce9cd69a))
* Relocated UsernameMapping tests into platform-independent test project ([70ee83c](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/70ee83c7485a1f7ead1a896eaa50d83eb6e00697))
* Remove double message generation ([669f7da](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/669f7dad9afe1779afedfd2bd96a74fd1c26bbd4))
* Removes unused directory ([1c29bae](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/1c29baed74d0a14dcccad87092b2867505a0f8b2))
* Resolved various Resharper warnings ([738503d](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/738503d9d227d04111b6a4e1afeea39d86bf6723))
* ToString() defaults to CharacterNameDisplay.WITH_WORLD to no longer break implicit calls inside logging ([1b14415](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/1b1441523aa73c68de3e626ff78aac7e200222fb))
* **README:** Adds code coverage badge [skip ci] ([c84e303](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/c84e303a6eb9fe021cfef1e318820ac9e5385560))

<a name="1.2.0"></a>
## [1.2.0](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v1.2.0) (2023-7-22)

### Features

* Handle emoji, mentions, attachments and stickers ([7159595](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/715959573397dc9b7d0c43daada4a698daf40b3c))

<a name="1.1.0"></a>
## [1.1.0](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v1.1.0) (2023-7-22)

### Features

* Increases coverage ([5da2cf9](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/5da2cf9f833ac45ca74d98039ff15f77a91d04b2))

### Other

* Utilizes Dependency Injection to allow to easily split byte parser into separate project with test coverage ([092a530](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/092a530d2e8ec7267a906ceec00096752db959b0))

<a name="1.0.0"></a>
## [1.0.0](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/releases/tag/v1.0.0) (2023-7-19)

### Features

* **Producer/FFXIV:** Initial commit ([67e2ab2](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/67e2ab217f225155e50c9786bcb9bb474b1cf730))

### Breaking Changes

* **Producer/FFXIV:** Initial commit ([67e2ab2](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/67e2ab217f225155e50c9786bcb9bb474b1cf730))

### Other

* Applies various Rider quick-fixes ([0be7bb5](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/0be7bb5eaece64d949245f751d197cc5224c3d7f))
* Forward all first exceptions first chance ([7aca52c](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/7aca52c883263ac850ce2979e2b916a38ddae695))
* Updates README setup instruction and notices ([7c30d9e](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/7c30d9e86753fa915ac483f464b28f8c5320879f))
* Use DI to allow for easier testing ([a0a626d](https://www.github.com/ViMaSter/FFXIVDiscordChatBridge/commit/a0a626dc59ed8783f78253ba9d8a1d0c8c35f041))

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

