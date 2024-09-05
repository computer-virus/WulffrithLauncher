# Wulffrith Launcher 1.4
A launcher application for my friend which I pushed to a repo so he can see the progress.
The goal is to make the launcher work relatively closely to the Windows 10 tiled start menu.

The launcher has gotten to a point where it is useable enough for me to be satisfied with it.
Which means, this is the final update I will be doing on this project for a good while.
That said, I may get to the rest of the implementations on the roadmap but I don't see these as necessary right now and I'd like to move on to other things for the time being.

I worked really hard on this so I hope you like the launcher and can make some use of it.

## Launcher Roadmap

### Current Implementations:
- Semi-Transparent Background
- Always Topmost Window
- Directory For App Data Storage
- Loading Data From Files Into Temporary Memory
- Launcher Window Is Fixed To Requested Location
- Automatically Minimizes On First Launch Or When Unfocused
- Launcher window scaling to be adaptive to screen size
- Loading Image Data From Directory Into Temporary Storage
- Validation For File Count
- Validation For Panel Size
- Validation For Image Loading
- Tested Different Children Properties (Using Children)
- Switched To using Much Simpler Grid System That Is More Accurate To What Friend Originally Wanted
- Use App Data And Image Data To Create Icons On The Grid System
- Added Grid Height Validation
- Added Check for Space in grids
- Added Image Validation
- Added Launcher Icon
- Added Ability For Launcher To Open Certain File Types Directly
- Switched To Reactive App Validation To Allow Greater Freedom And Simplicity For The User
- User Can Modify App Data and Image Data By Right-Clicking Icon
- Auto-run On Startup Compatibility
- Added Launcher Settings Bar

### Possible Future Implementations (If I Get To Them):
- Add manual launcher window scaling through in-app menu
- Alt Image On Hover Option
- Improved Indexing System And Allow Users To Drag Icons To Change App Indexes
- Implement Acrylic Brush Into WPF For Launcher Background
- Allow User To Add New App Data Through The Launcher Rather Than Directory

## Bug Tracker

### Known Unfixed Bugs:
- None so far

### Fixed Bugs:
- App still crashes if you don't have the image file in folder but reference it in appdata
- Asks User To Use A Shortcut Which Doesn't Work (Reworked Path System)
- File Explorer Will Launch A New Instance Of Itself And Won't Close (Workaround now has you not launch these support apps directly.)
- Fixed a bug that caused app icons to shrink near the border of the grid

### Report Bugs:
You can either send bug reports through this repository or to @computer_virus. on Discord.

## Feature Recommendations

### Contact Information
Any additional feature recommendations can be sent to @computer_virus. on Discord or through this repository.
If I do add your recommended feature (or plan to add it), you'll see it listed in the Launcher Roadmap above.
Be aware that you won't see it be added to this repository for a while during the time I take my break.

### Feature Restrictions
Note that I may not add your feature recommendation if it would vastly exceed my budgeted time and effort for the project or if it would be inappropriate.
Additionally, any features that require some purchase (whether it be one-time, licence, or otherwise) are off the table as well.
I don't want to pay money to make this launcher and I certainly don't want you to pay money have this launcher made either.
If a traditionally paid feature seems easy enough for me to replicate for free, I will consider adding it to roadmap.

## Additional Launcher Information

### Access
I plan on keeping the launcher free and open-source to the public at all times.
That means from the entirety of its development to well after the finalized build is published.
You are by all means allowed to download and modify the launcher to your heart's content.
You also may distribute this launcher or a modified copy of this launcher granted two things:
- Any modifications you or somebody else made to that copy of launcher are not malicious in any shape way or form.
- You keep all your distributions of the launcher 100% free and open-source to the public at all times.

### Authors
As of writing this, I'm the only one working on code for this project.
My friend has given me his suggestions in regards to the UI and features, many of which you'll see on the roadmap.
If you're interested in aiding me in the project, you can contact @computer_virus. on Discord or through this repository.
I'll let you know when I plan to start working on this again and we can start on some stuff.

### Future Projects
I do not currently plan on making many more publicly available projects for a while as I am currently going through post-secondary.
That said, I will still be making projects and you can expect them sometime after I'm finished with my schooling; one of those projects being the open-source code to the library I used for this project.
Any recommendations for future projects are appreciated and can be sent to @computer_virus. on Discord or through this repository.
I may not get to doing all of them but I will try to at least do the ones that are interesting.