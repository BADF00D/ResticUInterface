# Brainstorming

## Aim

* should support repositories on VeryCrypt container
* Detect HDD, ask for password and perform
  * integrity check
  * backup
* if running in background and HDD is connected permanently
  * runs scheduled backgrounds
  * mounts VeraCrypt containers only during background
* if running in background and no HDD is connected
  * schedules demands for backups
* backups configuration to repositories
* associates a HDD with a certain backup to run
* optional support for MFA like Yubikey
* Name proposal: HateBackup

## Requirements

### VeryCrypt
* Path to VeryCrypt

### Restic
* Path to Restic

## Architecture

* The core program that manages the configures and runs backups, should be platform independent
* The core program should provides extensions points
  * to detect HDD platform 
  * to mount VeryCrypt volumes
* The core program has a REST-API 
  * to configure backups
  * watch backups
  * inspect Restic repositories
  * (later) restore data from repository
* 