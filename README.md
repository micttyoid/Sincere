# Sincere: A lightweight PDF viewer as Blazor component

## Goal

1. Minimize interop footprint
2. Provide self-reliance from UI library
3. Provide versatility in applying UI library

## Design

1. Keep it minimum

    - Minimize number of dependencies

2. No overkill

    - Abuse of generic/template does not mean the versatility

3. NO version push of .NET unless "Leading Dependee" is about to do so
    - The current Leading Dependee is 
    [here](https://github.com/jlind0/Programming.Team)
    - This helps the low-maintenance

## Motivation

We wanted the PDF feature only from the Bootstrap library for the 
[app](https://github.com/jlind0/Programming.Team). We also want more PDF 
feature on top of it.

## Credit

The utilization of PDF.js in this project was adapted from 
[Vikram Reddy](https://github.com/gvreddy04). You can find the original code 
[here](https://github.com/vikramlearning/blazorbootstrap).
