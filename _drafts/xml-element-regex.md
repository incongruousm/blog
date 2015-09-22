# Regex to match XML Element & Content
- Jason Deabill
- incongruousm
- 2013-07-26T11:01:00+00:00
- Tech
- aside

A quick regex to match open- and close-element tags in XML and the intermediate content, where _element_ is the tag name.

```regex
^(.*<element.*)$(.|\n)*?(<\/element>.*)$\n
```