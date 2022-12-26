# Tasks to do with Descritions

## Develop General Handlers

### Debugging

- It shouldbe possible to write Debug Messages to the Detailed Info of the PB.
	- Done

### Argument Handler

- It should be able to read the argument of the PB, and choose the correct route based on it. See: [Link](https://github.com/malware-dev/MDK-SE/wiki/Handling-Script-Arguments)
	- Done 
- It should be able to trigger multiple routes with the same argument
	- This should be customizable

### Config Handler

- It should use the built in Custom Data Config Handler solution of SE. See: [Link](https://github.com/malware-dev/MDK-SE/wiki/Handling-configuration-and-storage)
	- Done
- It should handle data so only the custom fields are maintained in the config.
	- Done
- It should be able to read data from the Custom Data of the PB.
	- Done
- It should be able to initialize the custom data of the PB.
	- Done

### State Saving

- It should be able to store a set of primitive variables and their values. See: [Link](https://github.com/malware-dev/MDK-SE/wiki/The-Storage-String)
	- Done
- It should be able to load and assign data from the storage string.
	- Done

### Automatic Running

- It should be able to schedule an automatic run at any frequency. (Including NONE) See: [Link](https://github.com/malware-dev/MDK-SE/wiki/Continuous-Running-No-Timers-Needed)
	- Done
- Multiple frequencies might be scheduled at the same time.
	- Done
- It should be able to detect which frequencies triggered the actual run.
	- Done

### Block Manager

- It should use the recommended structuro for getting blocks. See [Link](https://github.com/malware-dev/MDK-SE/wiki/The-Grid-Terminal-System)
- It should be able to load required blocks and distribute them internally for the different parts of the script.
- It shouldbe able to distingush blocks based on a main tag
- It should be able to load blocks of specific types instead of based on name

### Screen Message Handler

- It should be able to display messages on any number of LCD Screens
- It should be able to find nested LCD screens in any block using a custom syntax in the CustomData

## Mining Platform Code

### Platform Argument Router

- Contains the main commands the platform recognises

#### Set

- Reloads The Config for The Platform
	- Done
- Repopulatess all blocks for the platform
- Reinitializes:
	- Step Manager
	- Piston Arms
	- Rotation Handlers
- Displays Errors regarding the platform
- When everything looks fine, then updates the state of the platform to ready to start
	- WIP
- Moves the Piston Arms to the designated starting position
- Moves the Rotation Handler to the designated starting position
- Args:
	- [1]Optional, the step number that the platform should move to
		- WIP
- Flags:
	- `-m` instructs the parser to use the [1] argument as the depth of the initial load for the platform in meters
		- WIP
	- `-dig` instructs the platform to handle the mining sequence as a dig instead of mine
		- WIP

#### Refresh

- Reloads and applies the Config from the Custom Data for soft values
- Reloads the soft blocks of the platform
- Flags:
	- `-hard`: Reloads the hard values and hard blocks as well
		- If an inconsistent state occures for the platform, then it is stopped

#### Start

- Checks if the platform is ready to be started
	- If not, then Writes an error
- Initiates an automatic run
	- If the platform is already running, then simply unpauses the run manager
- Starts the mining sequence
	- If the Pison Arm or the rotation is Stopped, then that is re-started
- Updates the state of the platform to being started

#### Pause

- Pauses the Run Manager
- Stops all Piston Movements
- Stops all Rotations
- Updates the Platform status to be Paused

#### Reset

- Clears the Custom Data and the Storage string, and resets everything to it's initial state

### Step Manager



### Piston Arm Handler

### Rotation Handler

### Data and Info Display

### Display Data Transmitter

### Event Triggers