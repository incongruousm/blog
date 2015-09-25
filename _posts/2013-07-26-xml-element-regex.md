---
layout: post
title:  "Regex to match XML Element & Content"
date:   2013-07-26T11:01:00+00:00
tags:   tech
---

A quick regex to match open- and close-element tags in XML and the intermediate content, where `element` is the tag name.

```
^(.*<element.*)$(.|\n)*?(<\/element>.*)$\n
```
