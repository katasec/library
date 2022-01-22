# Overview


Create stack

```
make up
```

Destroy stack

```
make destroy
```



# What this module should do 

This `base` module needs to create:


|Resource Group | Storage Account | Container Name | Remarks |
|-|-|-|-|
| base | stbase\<company short name\>.\<random string\> | mgmt | For e.g. Storage account name is: `stbaseks12345`|
| base | stbase.\<company short name\>.\<random string\> | dev | For e.g. Storage account name is: `stbaseks12345`|
| base | stbase.\<company short name\>.\<random string\> | uat | For e.g. Storage account name is: `stbaseks12345`|
| base | stbase.\<company short name\>.\<random string\> | prod | For e.g. Storage account name is: `stbaseks12345`|
| base | stbase.\<company short name\>.\<random string\> | iam | For e.g. Storage account name is: `stbaseks12345`|
| base | stbase.\<company short name\>.\<random string\> | adds | For e.g. Storage account name is: `stbaseks12345`|
| base | stbase.\<company short name\>.\<random string\> | er | For e.g. Storage account name is: `stbaseks12345`|