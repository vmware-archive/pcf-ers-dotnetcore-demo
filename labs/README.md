# PCF Elastic Runtime Basic Demo Labs
A set of labs that help you understand PCF and this application. 
You can run them in no particular order, but below is our suggestion. 

Tested with PCF 1.11, PCFDev, Pivotal Web Services and PEZ (Internal Pivotal Environment)

* [PCF PUSH](https://github.com/Pivotal-Field-Engineering/pcf-ers-demo/blob/master/Labs/Application_Push/lab_application_push.adoc)
* [Logging, Scale and HA](https://github.com/Pivotal-Field-Engineering/pcf-ers-demo/blob/master/Labs/Logging_Scale_HA/lab_logging_scale_ha.adoc)
* [Services](https://github.com/Pivotal-Field-Engineering/pcf-ers-demo/blob/master/Labs/Services/lab_services.adoc)
* [Blue Green Deployment](https://github.com/Pivotal-Field-Engineering/pcf-ers-demo/blob/master/Labs/Blue_Green/lab_blue_green.adoc)
* [Buildpacks](https://github.com/Pivotal-Field-Engineering/pcf-ers-demo/blob/master/Labs/Buildpacks/lab_buildpack.adoc)
* [Spring Boot Integration ]()

## Workshop Site

We're working on using asciidoctor to generate an simple workshop site.

```
asciidoctor "**/*.adoc" -D site
```