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
- It should handle data so only the custom fields are maintained in the config.
- It should be able to read data from the Custom Data of the PB.
- It should be able to initialize the custom data of the PB.

### State Saving

- It should be able to store a set of primitive variables and their values. See: [Link](https://github.com/malware-dev/MDK-SE/wiki/The-Storage-String)
- It should be able to load and assign data from the storage string.

### Automatic Running

- It should be able to schedule an automatic run at any frequency. (Including NONE) See: [Link](https://github.com/malware-dev/MDK-SE/wiki/Continuous-Running-No-Timers-Needed)
- Multiple frequencies might be scheduled at the same time.
- It should be able to detect which frequencies triggered the actual run.

### Screen Message Handler

