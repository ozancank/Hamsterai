import 'package:camera/camera.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/scan/controller/scan_controller.dart';
import 'package:mobile/styles/colors.dart';

class ScanPage extends StatefulWidget {
  const ScanPage({super.key});

  @override
  State<ScanPage> createState() => _ScanPageState();
}

class _ScanPageState extends State<ScanPage> {
  late ScanController scanController;

  @override
  void initState() {
    super.initState();
    scanController = Get.find<ScanController>();
    scanController.initializeCamera();
    scanController.getLessons(() {
      WidgetsBinding.instance.addPostFrameCallback((_) async {
        await _showOverlayPage();
      });
    }, context);
  }

  @override
  void dispose() {
    scanController.cameraController.dispose();
    super.dispose();
  }

  _showOverlayPage() {
    showGeneralDialog(
      context: context,
      barrierDismissible: false,
      barrierColor: Colors.black.withOpacity(0.5),
      transitionDuration: const Duration(milliseconds: 300),
      pageBuilder: (context, animation, secondaryAnimation) {
        return WillPopScope(
          onWillPop: () async {
            Get.offAllNamed('/home');
            return false;
          },
          child: Scaffold(
            backgroundColor: Colors.black.withOpacity(0.5),
            body: Obx(
              () {
                if (scanController.isLoading.value) {
                  return const Center(child: CircularProgressIndicator());
                }
                return Column(
                  children: [
                    Align(
                      alignment: Alignment.topRight,
                      child: Padding(
                        padding: EdgeInsets.only(
                          top: context.dynamicHeight * 0.05,
                          right: context.width * 0.1,
                        ),
                        child: Container(
                          padding: const EdgeInsets.all(10),
                          decoration: const BoxDecoration(
                              shape: BoxShape.circle, color: Colors.white),
                          child: GestureDetector(
                            onTap: () {
                              Get.offAllNamed('/home');
                            },
                            child: Icon(
                              Icons.close_rounded,
                              color: MyColors.primaryColor,
                              weight: 400,
                              size: context.height * 0.04,
                            ),
                          ),
                        ),
                      ),
                    ),
                    Expanded(
                      child: ListView.builder(
                        itemCount: scanController.lessons.length,
                        itemBuilder: (context, index) {
                          return Container(
                            alignment: Alignment.centerLeft,
                            padding: const EdgeInsets.all(6),
                            child: Column(
                              mainAxisSize: MainAxisSize.min,
                              children: [
                                Container(
                                  padding: EdgeInsets.only(
                                    left: 4,
                                    right: context.width * 0.08,
                                    top: context.height * 0.003,
                                    bottom: context.height * 0.003,
                                  ),
                                  margin: EdgeInsets.symmetric(
                                      horizontal: context.width * 0.05),
                                  decoration: BoxDecoration(
                                    color: MyColors.primaryColor,
                                    borderRadius: BorderRadius.circular(40),
                                  ),
                                  child: GestureDetector(
                                    onTap: () {
                                      HapticFeedback.mediumImpact();
                                      Get.back();
                                      scanController.lessonId.value =
                                          scanController.lessons[index].id;
                                    },
                                    child: Row(
                                      mainAxisSize: MainAxisSize.min,
                                      mainAxisAlignment:
                                          MainAxisAlignment.start,
                                      children: [
                                        Container(
                                          padding: const EdgeInsets.all(14),
                                          decoration: const BoxDecoration(
                                            color: Colors.white,
                                            shape: BoxShape.circle,
                                          ),
                                          child: Center(
                                            child: SvgPicture.asset(
                                              AssetsConstant.rightBack,
                                              color: MyColors.primaryColor,
                                              height: context.height * 0.02,
                                            ),
                                          ),
                                        ),
                                        const SizedBox(width: 10),
                                        Text(
                                          toTurkishUpperCase(scanController
                                              .lessons[index].name),
                                          style: Theme.of(context)
                                              .textTheme
                                              .titleLarge!
                                              .copyWith(
                                                color: Colors.white,
                                                overflow: TextOverflow.ellipsis,
                                                fontWeight: FontWeight.w500,
                                              ),
                                        ),
                                      ],
                                    ),
                                  ),
                                ),
                              ],
                            ),
                          );
                        },
                      ),
                    ),
                  ],
                );
              },
            ),
          ),
        );
      },
    );
  }

  String toTurkishUpperCase(String input) {
    return input
        .replaceAll('i', 'İ')
        .replaceAll('ı', 'I')
        .replaceAll('ü', 'Ü')
        .replaceAll('ö', 'Ö')
        .replaceAll('ç', 'Ç')
        .replaceAll('ş', 'Ş')
        .replaceAll('ğ', 'Ğ')
        .toUpperCase();
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      body: Stack(
        children: [
          Obx(() {
            if (scanController.shouldShowOverlay.value) {
              WidgetsBinding.instance.addPostFrameCallback((_) {
                _showOverlayPage();
                scanController.shouldShowOverlay.value = false;
              });
            }

            if (scanController.isLoading.value) {
              return const Center(child: CircularProgressIndicator());
            }
            if (scanController.isCameraInitialized.value) {
              return Positioned.fill(
                child: CameraPreview(scanController.cameraController),
              );
            } else if (scanController.hasCameraError.value) {
              return const Center(
                  child: Text('Kamera başlatılırken bir hata oluştu'));
            } else {
              return const Center(child: CircularProgressIndicator());
            }
          }),
          Align(
            alignment: Alignment.topRight,
            child: Padding(
              padding: EdgeInsets.only(
                top: context.dynamicHeight * 0.05,
                right: context.width * 0.05,
              ),
              child: Container(
                padding: const EdgeInsets.all(10),
                decoration: const BoxDecoration(
                  shape: BoxShape.circle,
                  color: MyColors.primaryColor,
                ),
                child: GestureDetector(
                  onTap: () async {
                    await scanController.toggleFlash();
                  },
                  child: Obx(() {
                    return Container(
                      decoration: const BoxDecoration(
                        color: MyColors.primaryColor,
                        shape: BoxShape.circle,
                      ),
                      child: Icon(
                        scanController.isFlashOn.value
                            ? Icons.flash_on
                            : Icons.flash_off,
                        color: Colors.white,
                        size: context.height * 0.04,
                      ),
                    );
                  }),
                ),
              ),
            ),
          ),
          Align(
            alignment: Alignment.bottomCenter,
            child: Padding(
              padding: EdgeInsets.only(bottom: context.dynamicHeight * 0.01),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  Container(
                    padding: EdgeInsets.symmetric(
                        vertical: context.height * 0.01,
                        horizontal: context.width * 0.1),
                    margin: const EdgeInsets.symmetric(horizontal: 10),
                    decoration: BoxDecoration(
                      color: Colors.black.withOpacity(0.1),
                      borderRadius: BorderRadius.circular(30),
                    ),
                    child: Text(
                      scanController.layoutMode.value
                          ? 'Önce paragrafı çekin, ardından soruyu çekin.'
                          : 'Kamerayı sorunun olduğu alana tutun ve sadece tek soru çekin.',
                      textAlign: TextAlign.center,
                      style: Theme.of(context).textTheme.titleSmall!.copyWith(
                          color: Colors.black, fontWeight: FontWeight.w300),
                    ),
                  ),
                  SizedBox(
                    height: context.height * 0.03,
                  ),
                  Row(
                    children: [
                      Expanded(
                        child: GestureDetector(
                          onTap: () {
                            scanController.pickImageFromGallery(context);
                          },
                          child: Container(
                            alignment: Alignment.center,
                            padding: const EdgeInsets.all(15),
                            decoration: const BoxDecoration(
                              shape: BoxShape.circle,
                              color: MyColors.primaryColor,
                              //  color: Color(0xFFD9D9D9),
                            ),
                            child: SvgPicture.asset(
                              AssetsConstant.gallery,
                              color: Colors.white,
                            ),
                          ),
                        ),
                      ),
                      Expanded(
                        child: GestureDetector(
                          onTap: () async {
                            if (scanController.layoutMode.value) {
                              if (scanController.firstImageFile != null) {
                                scanController.captureSecondImage(
                                    context, scanController.firstImageFile!);
                              } else {
                                scanController.onLayerIconTapped(context);
                              }
                            } else {
                              await scanController.takePicture(context);
                            }
                          },
                          child: SvgPicture.asset(
                            AssetsConstant.scan,
                            height: context.dynamicHeight * 0.1,
                          ),
                        ),
                      ),
                      Expanded(
                        child: GestureDetector(
                          onTap: () {
                            setState(() {
                              scanController.layoutMode.value =
                                  !scanController.layoutMode.value;
                            });
                          },
                          child: Container(
                            alignment: Alignment.center,
                            padding: const EdgeInsets.all(15),
                            decoration: const BoxDecoration(
                              shape: BoxShape.circle,
                              color: MyColors.primaryColor,
                              //color: Color(0xFFD9D9D9),
                            ),
                            child: const Icon(
                              Icons.layers,
                              color: Colors.white,
                            ),
                          ),
                        ),
                      ),
                    ],
                  ),
                ],
              ),
            ),
          ),
        ],
      ),
    );
  }
}
