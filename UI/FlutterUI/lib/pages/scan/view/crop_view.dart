import 'dart:io';
import 'dart:typed_data';

import 'package:crop_your_image/crop_your_image.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter/material.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
import 'package:get/get.dart';
import 'package:mobile/pages/scan/controller/scan_controller.dart';
import 'package:mobile/styles/colors.dart';

import 'package:path_provider/path_provider.dart';

class CropView extends StatefulWidget {
  const CropView(
      {super.key,
      required this.image,
      this.isSecondToGo = false,
      required this.doublePhoto});
  final Uint8List image;
  final bool isSecondToGo;
  final bool doublePhoto;
  @override
  State<CropView> createState() => _CropViewState();
}

class _CropViewState extends State<CropView> {
  final _controller = CropController();
  Uint8List? croppedImage;
  final scanController = Get.put(ScanController());

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          Center(
            child: Crop(
              image: widget.image,
              controller: _controller,
              onCropped: (Uint8List croppedImage) async {
                if (croppedImage != null) {
                  try {
                    final directory = await getTemporaryDirectory();
                    final fileName =
                        "cropped_image_${DateTime.now().millisecondsSinceEpoch}.png";
                    final newPath = '${directory.path}/$fileName';
                    final newImageFile = File(newPath);
                    await newImageFile.writeAsBytes(croppedImage);

                    if (widget.isSecondToGo) {
                      scanController.firstImageFile = newImageFile;
                      scanController.pickSecondImage.value = true;

                      EasyLoading.showInfo('Şimdi 2. fotoğrafı çekiniz');
                      Get.back();
                    } else if (widget.doublePhoto) {
                      scanController.secondImageFile = newImageFile;
                      scanController.pickSecondImage.value = false;
                      scanController.mergeImagesAndSendToAPI(context);
                      //Get.back();
                    } else {
                      scanController.compressImageToBase64(
                          newImageFile, context);
                    }
                  } catch (e) {
                    if (kDebugMode) {
                      print("Görüntü dosyaya kaydedilemedi: $e");
                    }
                  }
                }
              },
              aspectRatio: null,
              initialSize: 1.0,
              baseColor: Colors.black,
              //  maskColor: Colors.white.withAlpha(100),
              progressIndicator: const CircularProgressIndicator(),
              radius: 10,
              onMoved: (newRect) {},
              onStatusChanged: (status) {},
              cornerDotBuilder: (size, edgeAlignment) => const DotControl(
                color: MyColors.primaryColor,
                padding: 3,
              ),
              clipBehavior: Clip.none,
              interactive: false,
              initialRectBuilder: (viewportRect, imageRect) {
                return Rect.fromLTRB(
                  viewportRect.left + (viewportRect.width / 16),
                  viewportRect.top + (viewportRect.height / 3),
                  viewportRect.right - (viewportRect.width / 16),
                  viewportRect.bottom - (viewportRect.height / 3),
                );
              },
            ),
          ),
          Positioned(
            top: 60,
            left: 20,
            child: Container(
              padding: const EdgeInsets.all(10),
              decoration: const BoxDecoration(
                shape: BoxShape.circle,
                color: MyColors.primaryColor,
              ),
              child: GestureDetector(
                onTap: () async {
                  Get.back();
                },
                child: Container(
                  decoration: const BoxDecoration(
                    color: MyColors.primaryColor,
                    shape: BoxShape.circle,
                  ),
                  child: Icon(
                    Icons.close_rounded,
                    color: Colors.white,
                    size: context.height * 0.04,
                  ),
                ),
              ),
            ),
          ),
          Positioned(
            bottom: 40,
            right: 40,
            child: InkWell(
              onTap: () async {
                EasyLoading.show(
                    status: 'Soru işleniyor. Lütfen bekleyiniz...');
                _controller.crop();
              },
              child: Container(
                padding: EdgeInsets.only(
                    right: context.width * 0.08, top: 5, bottom: 5, left: 5),
                decoration: BoxDecoration(
                  color: MyColors.primaryColor,
                  borderRadius: BorderRadius.circular(40),
                ),
                child: Row(
                  mainAxisSize: MainAxisSize.min,
                  children: [
                    Container(
                        height: context.height * 0.06,
                        alignment: Alignment.center,
                        padding: const EdgeInsets.all(5),
                        decoration: const BoxDecoration(
                          color: Colors.white,
                          shape: BoxShape.circle,
                        ),
                        child: const Icon(
                          Icons.crop_outlined,
                          color: MyColors.primaryColor,
                        )),
                    const SizedBox(width: 10),
                    Text(
                      'KIRP',
                      style: Theme.of(context).textTheme.bodyLarge!.copyWith(
                            color: Colors.white,
                            fontWeight: FontWeight.w900,
                          ),
                    ),
                  ],
                ),
              ),
            ),
          ),
        ],
      ),
    );
  }
}
