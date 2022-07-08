#Deployment Procedure

#### Requirements:
- Should be single .exe probably with config
- Should be packed with UPX/Somekind of packer

#### Flow
1. Release Compile
    - Check Settings
3. UPX Packed
    - De Check
2. Move to custom folder
4. Usability Test
1. Workshop Upload
    1. Criteria
        1. Tag ?
        1. Category ?


#### Flow v2
1. Release Compile
    - Assembly Version Check
    - Assembly FileVersion Check
    - Donation Link check.
2. Packer
    - Smart Assembly 8
    - VirusTotal
    - check with dnSpy/dotPeek/.NET Reflector
3. False detection check
4. Move to Custom folder release
5. Usability Test
    - Loader working properly
    - API off/on properly
    - Metrics shared properly
    - Editor working properly
    - Mini Editr working properly
6. Workshop Upload
    1. Criteria
        1. Tag ?
        1. Category ?