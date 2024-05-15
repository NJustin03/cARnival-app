---
uid: arcore-camera
---
# Camera

This page is a supplement to the AR Foundation [Camera](xref:arfoundation-camera) manual. The following sections only contain information about APIs where ARCore exhibits unique platform-specific behavior.

[!include[](../snippets/arf-docs-tip.md)]

## Camera configuration

[XRCameraConfiguration](xref:UnityEngine.XR.ARSubsystems.XRCameraConfiguration) contains an `IntPtr` field `nativeConfigurationHandle`, which is a platform-specific handle. For ARCore, this handle is the pointer to the `ArCameraConfiguration`. The native object is managed by Unity. Do not manually destroy it.

## EXIF data

This package implements AR Foundation's [EXIF data](xref:arfoundation-exif-data) API using ARCore's [ArImageMetadata](https://developers.google.com/ar/reference/c/group/ar-image-metadata#arimagemetadata). Refer to the following table to understand which tags ARCore supports:

| EXIF tag                | Supported |
| :---------------------- | :-------: |
| ApertureValue           | Yes |
| BrightnessValue         |     |
| ColorSpace              |     |
| ExposureBiasValue       |     |
| ExposureTime            | Yes |
| FNumber                 | Yes |
| Flash                   | Yes |
| FocalLength             | Yes |
| PhotographicSensitivity | Yes |
| MeteringMode            |     |
| ShutterSpeedValue       | Yes |
