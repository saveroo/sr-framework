#TODO List

#### Must Be:
- Able to load different PTR class 
less painfully

#### Fix:
- Load Flow Consistency
- Less tight coupling
- Army Experience PTR have diff address (this will be troublesome for grouping feature)

#### Refactor:
- Pure PTR Class
- Pure Function Wrapper Class

### Testability:
- [ ] Unit Test
- [ ] Integration Test
- [ ] Loader Test
- [ ] API Test
  - [ ] When it's on/off
  
### Known Bugs:
- [ ] When Ads API Blacklist/Whitelist call is failure, it will crash the program
- [ ] RESERVED
- [ ] RESERVED

### Schema/Metadata TODO:
- [x] SubCategory (v3)
  - Included Features (v3)
- [x] Added 1 day Rail/Road Feature (v3)
- [x] Added Facility Outputs Feature (v3)
- [x] Added Resource Outputs Multiplier Feature (v3)

### API TODO:
- [x] Device Registration /client 
- [x] Version check /check
- [x] Metadata Download /data
- [x] Steam API Bridge /steam (v3)
- [x] Donator Ads whitelist/blacklist /s (v3)


#### Client TODO:
- General
  - [x] API Whitelist/Blacklist Ads Viewer (v3)
  - [x] Extract Hyperlink from feature.description and populate into wiki fields (v3)
- [x] [Form] SR Loader
  - [ ] Offline capability (WIP)
  - [x] Fetch from API
  - [x] Encryption/Decryption Method
  - [x] Device Registration
  - [x] Check for update
  - [x] Pass METADATA to SR Form
  - [x] Properly registering device [API] (v3)
  - [x] Properly fetch Steam Users [API] (v3)
  - [x] UI UpdateState to prevent error (v3)
- [x] [Form] Mini Editor (v3)
  - [x] Copied most of the controls, excluding (Facility build time, Research Efficiency) (v3)
  - [ ] [Test] Sync values with SR Form controls equivalent (v3)
  - [ ] [Test] Grid View bound to the same DataSource as mainform (v3)
  - [x] Winhook API (v3)
  - [x] Synchronized Drag (v3)
  - [x] Opacity when out of focus. (TODO: Need usability testing) (v3)
  - [x] Ability to drag the form (v3)
- [x] [Form] SR Main Form
  - [x] Open/Exit Mini Editor Form (v3)
  - [ ] Locationed Interstitial Ads (v3)
  - [ ] Time Based Interstitial Ads (v3)
  - [x] GridView separated to subcategory (v3)
  - [x] Rewrite UI with DevExpress
  - [x] [Tab] Country Tab
    - [x] Freeze Social Spending Button (v3)
  - [x] [Tab] Resource Tab
    - [x] Freeze All Button (v3)
  - [x] [Tab] Users (v3)
    - [x] SteamRun Button API (v3)
    - [x] Played Time (v3)
    - [x] Multiple Steam WEB API Button (Open Map/Content Editor) (v3)
    - [ ] SR Players List (In Consideration) (v3)
    - [x] STEAM Avatar (v3)
    - [x] STEAM Name (v3)
  - [x] [Tab] Warfare
    - Main Warfare Tab
      - [x] Class/Unit/Move
      - [x] Special Option (Rambo ETC)
      - [x] Unit History
      - [x] Health Bar as in game
    - ModifiedUnit Tab
      - [x] Automatically add unit to modified unit list (v3)
      - [x] Unit can be restored to its original state (v3)
      - [ ] Use global store to store unit (v3)
    - PersistentUnit/UnitTracker Tab
      - [x] Persistent/Tracked Unit List (v3)
      - [x] Stats can be edited (v3)
      - [x] Can be removed (v3)
  - [x] [Tab] HTML Browser Tab 
    - [x] Show Cheats
    - [ ] Separate tabs to show wiki, forum, etc. (v3)
  - [x] [Tab] Special
    - [x] Facilities Editor
      - [ ] Ability to select edited facility when it's not selected in game (v3) 
      - [x] Replaced from form to GridView to display (v3)
      - [x] [new] feature facilities max output (v3)
    - [ ] 1 Day Rail/Road Build (v3) (WIP)
    - [x] 1 Day Facility Build
    - [x] 1 Day Army Build
    - [x] 1 Day Research
  - [x] [Main] About Tab
    - [x] Changelog Information
    - [x] Product Information
    - [x] Help Information
    - [x] Donation Button
- App flow
  - Loader > MainForm > ReadWrite > Unit/Info/Res > PTR Class > fetch from JSON
- Feature
  - [x] Informational Display (Domestic/Country)
  - [x] Unit Editor (Persistent/Unique)
  - [x] Resource Editor
  - [x] Domestic/Social/Recon(FOW)
  - [x] Missile multiplier (all interface)
  - [x] Cheats List

#### Missing (Not implemented yet):
- Enemy Unit Editor (PTR)
- Production Cost/Price (PTR)
- New Update Check Online XML;
- Donator List
- Web Browser wiki ?

#### Requirement:
- RW.cs (Should be single instance)
  - Depends on Memories.cs with IMemories interface
- Loader.cs 
  - Need to instantiate RW.cs
  - Pass to Mainform, Information, Uniteditor.
  
#### Polish: