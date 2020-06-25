#TODO List

#### Must Be:
- Able to load different PTR class 
less painfully

#### Fix:
- Load Flow Consistency
- Less tight coupling
- Army Experience PTR have diff address (this will be troublesome for grouping feature)

#### Done:
- App flow
  - Loader > MainForm > RW > Unit/Info/Res > PTR Class > fetch from JSON
- Feature
  - Information Display (Domestic/Country)
  - Unit Editor (Persistent/Unique)
  - Resource Editor
  - Domestic/Social/Recon(FOW)
  - Missile multiplier (all interface)

#### Missing (Not implemented yet):
- Enemy Unit Editor (PTR)
- Production Cost/Price (PTR)
- New Update Check Online XML;
- Donator List
- Web Browser wiki ?

#### Perhaps: 
- Use MetroFramework Form, 
- Eliminate Syncfusion

#### Requirement:
- RW.cs (Should be single instance)
  - Depends on Memories.cs with IMemories interface
- Loader.cs 
  - Need to instantiate RW.cs
  - Pass to Mainform, Information, Uniteditor.
  
#### Polish: