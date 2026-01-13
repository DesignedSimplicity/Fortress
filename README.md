# Fortress

Repository: https://github.com/DesignedSimplicity
Project: https://github.com/users/DesignedSimplicity/projects/4
Issues: https://github.com/DesignedSimplicity/Fortress/issues

## Patrol

### Create MD5 hash file
### Verify MD5 hash file
### Output XLSX report file

```
patrol create							Creates an md5 hash file named {DriveLetter|DirectoryName}_YYYYMMDD-HHMMSS.md5 recurisvelly for all files starting in current directory
patrol create --start C:\StartHere		Provide a specific directory rather than the current
patrol create --name MyName				Prefix the _YYYYMMDD-HHMMSS file with a different name
patrol create --hash sha512				Specifies one or more hashes to create for each file
patrol create --search *.*				Filters the file indexed using wildcards *.*
patrol create --report					Creates a full report in excel 
patrol create --index					Does not compute the hashs or create the hash file, will create a report if specified
patrol create --error					Stops on first error
patrol create --log						Creates a simple log file
patrol create --flat					Disables directory recursion
```


# Common Options
