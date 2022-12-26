# Tasks to do with Descritions

**Legend:**
- ✅ Done
- 🔄 Work In Progres

## Develop General Handlers

### Debugging

- It shouldbe possible to write Debug Messages to the Detailed Info of the PB. ✅

### Argument Handler

- It should be able to read the argument of the PB, and choose the correct route based on it. See: [Link](https://github.com/malware-dev/MDK-SE/wiki/Handling-Script-Arguments) ✅
- [Optional] It should be able to trigger multiple routes with the same argument
	- This should be customizable

### Config Handler

- It should use the built in Custom Data Config Handler solution of SE. See: [Link](https://github.com/malware-dev/MDK-SE/wiki/Handling-configuration-and-storage) ✅
- It should handle data so only the custom fields are maintained in the config. ✅
- It should be able to read data from the Custom Data of the PB. ✅
- It should be able to initialize the custom data of the PB. ✅

### State Saving

- It should be able to store a set of primitive variables and their values. See: [Link](https://github.com/malware-dev/MDK-SE/wiki/The-Storage-String) ✅
- It should be able to load and assign data from the storage string. ✅

### Automatic Running

- It should be able to schedule an automatic run at any frequency. (Including NONE) See: [Link](https://github.com/malware-dev/MDK-SE/wiki/Continuous-Running-No-Timers-Needed) ✅
- Multiple frequencies might be scheduled at the same time. ✅
- It should be able to detect which frequencies triggered the actual run. ✅

### Block Manager

- It should use the recommended structuro for getting blocks. See [Link](https://github.com/malware-dev/MDK-SE/wiki/The-Grid-Terminal-System)
- It should be able to load required blocks and distribute them internally for the different parts of the script.
- It shouldbe able to distingush blocks based on a main tag
- It should be able to load blocks of specific types instead of based on name

### Screen Message Handler

- It should be able to display messages on any number of LCD Screens
- It should be able to find nested LCD screens in any block using a custom syntax in the CustomData

## Mining Platform Code

### Platform Runner

- Can Start the Platform and update the related states ✅
- Can Stop the Platform and update the related states ✅
- Can Pause the Platform and update the related states ✅
- Can be Initialized based on previous states and continue the running ✅

### Platform Argument Router

- Contains the main commands the platform recognises 🔄
	- SET 🔄
	- REFRESH 
	- START 🔄
	- PAUSE 🔄
	- RESET 🔄

#### Set

- Reloads The Config for The Platform ✅
- Repopulatess all blocks for the platform
- Reinitializes:
	- Step Manager ✅
	- Piston Arms 
	- Rotation Handlers
- Displays Errors regarding the platform
- When everything looks fine, then updates the state of the platform to ready to start 🔄 (Needs Platform Construction)
- Moves the Piston Arms to the designated starting position
- Moves the Rotation Handler to the designated starting position
- Args:
	- [1]Optional, the step number that the platform should move to ✅
- Flags:
	- `-m` instructs the parser to use the [1] argument as the depth of the initial load for the platform in meters 🔄 (Needs Meter Decoding)
	- `-dig` instructs the platform to handle the mining sequence as a dig instead of mine ✅

#### Refresh

- Reloads and applies the Config from the Custom Data for soft values
- Reloads the soft blocks of the platform
- Flags:
	- `-hard`: Reloads the hard values and hard blocks as well
		- If an inconsistent state occures for the platform, then it is stopped

#### Start

- Checks if the platform is ready to be started 🔄 (Needs Validity Checking)
	- If not, then Writes an error ✅
- Initiates an automatic run ✅
	- If the platform is already running, then simply unpauses the run manager ✅
- Starts the mining sequence 🔄 (Waiting for Piston/Rotor controller)
	- If the Pison Arm or the rotation is Stopped, then that is re-started 🔄 (Waiting for Piston/Rotor controller)
- Updates the state of the platform to being started ✅

#### Pause

- Pauses the Run Manager ✅
- Stops all Piston Movements
- Stops all Rotations
- Updates the Platform status to be Paused ✅

#### Reset

- Clears the Custom Data and the Storage string, and resets everything to it's initial state ✅
- Stops Any Piston/Rotor Movement

### Step Manager

- Maintains the steps of the mining sequence, where it can be determined for each step if they are a vertical/horizontal/rotation steps. ✅
	- Steps Are:
		- Rotation
		- For Each Vertical Step:
			- For Each Horizontal Step:
				- Horizontal Extension
				- Rotation
			- Vertical Extension
			- Rotation
		- For Each Horizontal Step:
			- Horizontal Extension
			- Rotation
		
- It can be re-initialized ✅
- It can be fast forwarded to any step if needed ✅
- It can determine if the mining sequecne was finished. ✅
- It has a static step for aligning to starting position, and retracting when finished. ✅
- Can be Initialized based on previous states ✅

### Piston Arm Handler

### Rotation Handler

### Data and Info Display

### Display Data Transmitter

### Event Triggers
