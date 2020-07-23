package main

import (
	"fmt"
	"io/ioutil"
	"os"
)


func main() {
    jsonFile, err := os.Open("SRFeature.json")

    if(err != nil) {
        fmt.Println(err)
    }

    fmt.Println("Success")
    defer jsonFile.Close();

    byteValue, _ = ioutil.ReadAll(jsonFile)

    // var Autoreaload
}