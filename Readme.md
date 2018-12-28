# Introduction

An example of using azure functions to:

- Download resources
- Store it as blob
- Do a blob comparision if same then replace the old blob with the new
- Then deserialise object and store in the azure table store (though you are able to use any thing from Cosmos db or EF)
