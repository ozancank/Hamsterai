import 'package:camera/camera.dart';
import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_easyloading/flutter_easyloading.dart';
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

  bool isLessonsLoaded = false;

  Future<void> loadLessons() async {
    EasyLoading.show(
        status: 'Dersler yükleniyor', maskType: EasyLoadingMaskType.black);
    await scanController.getLessons(() {
      setState(() {
        isLessonsLoaded = true;
      });
    }, context);

    if (isLessonsLoaded) {
      EasyLoading.dismiss();
      await _showOverlayPage();
    }
  }

  @override
  void initState() {
    super.initState();
    scanController = Get.find<ScanController>();
    scanController.initializeCamera();
    // scanController.getLessons(() {
    //   WidgetsBinding.instance.addPostFrameCallback((_) async {
    //     await _showOverlayPage();
    //   });
    // }, context);
    WidgetsBinding.instance.addPostFrameCallback((_) => loadLessons());
  }

  @override
  void dispose() {
    scanController.cameraController?.setFlashMode(FlashMode.off);
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
                                      scanController.lessonType.value =
                                          scanController.lessons[index].type;
                                      print(scanController.lessons[index].type);
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
                                        Flexible(
                                          child: Text(
                                            scanController.lessons[index].name,
                                            style: Theme.of(context)
                                                .textTheme
                                                .titleLarge!
                                                .copyWith(
                                                  color: Colors.white,
                                                  overflow:
                                                      TextOverflow.ellipsis,
                                                  fontWeight: FontWeight.w500,
                                                ),
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
            if (scanController.isCameraInitialized.value &&
                scanController.cameraController != null &&
                scanController.cameraController!.value.isInitialized) {
              return LayoutBuilder(
                builder: (context, constraints) {
                  final scale = 1 /
                      (scanController.cameraController!.value.aspectRatio *
                          (constraints.maxWidth / constraints.maxHeight));
                  return Transform.scale(
                    scale: scale,
                    child: Center(
                      child: CameraPreview(scanController.cameraController!),
                    ),
                  );
                },
              );
            } else if (scanController.hasCameraError.value) {
              return const Center(
                  child: Text('Kamera başlatılırken bir hata oluştu'));
            } else {
              return const Center(child: CircularProgressIndicator());
            }
          }),
          Positioned.fill(
              child: Center(
            child: SizedBox(
                width: context.width * 0.9,
                height: context.height * 0.3,
                child: Stack(
                  children: [
                    const Align(
                      alignment: Alignment.center,
                      child: Icon(
                        Icons.add,
                        color: MyColors.primaryColor,
                        size: 24,
                      ),
                    ),
                    Positioned(
                      top: 0,
                      left: 0,
                      child: Container(
                        width: 40,
                        height: 2,
                        color: MyColors.primaryColor,
                      ),
                    ),
                    Positioned(
                      top: 0,
                      left: 0,
                      child: Container(
                        width: 2,
                        height: 40,
                        color: MyColors.primaryColor,
                      ),
                    ),
                    Positioned(
                      top: 0,
                      right: 0,
                      child: Container(
                        width: 40,
                        height: 2,
                        color: MyColors.primaryColor,
                      ),
                    ),
                    Positioned(
                      top: 0,
                      right: 0,
                      child: Container(
                        width: 2,
                        height: 40,
                        color: MyColors.primaryColor,
                      ),
                    ),
                    Positioned(
                      bottom: 0,
                      left: 0,
                      child: Container(
                        width: 40,
                        height: 2,
                        color: MyColors.primaryColor,
                      ),
                    ),
                    Positioned(
                      bottom: 0,
                      left: 0,
                      child: Container(
                        width: 2,
                        height: 40,
                        color: MyColors.primaryColor,
                      ),
                    ),
                    Positioned(
                      bottom: 0,
                      right: 0,
                      child: Container(
                        width: 40,
                        height: 2,
                        color: MyColors.primaryColor,
                      ),
                    ),
                    Positioned(
                      bottom: 0,
                      right: 0,
                      child: Container(
                        width: 2,
                        height: 40,
                        color: MyColors.primaryColor,
                      ),
                    ),
                  ],
                )),
          )),
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
            alignment: Alignment.topLeft,
            child: Padding(
              padding: EdgeInsets.only(
                top: context.dynamicHeight * 0.05,
                left: context.width * 0.05,
              ),
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
          ),
          Align(
            alignment: Alignment.topCenter,
            child: Container(
              padding: EdgeInsets.symmetric(
                  vertical: context.height * 0.01,
                  horizontal: context.width * 0.1),
              margin: EdgeInsets.symmetric(vertical: context.height * 0.15),
              decoration: BoxDecoration(
                color: Colors.black.withOpacity(0.3),
                borderRadius: BorderRadius.circular(20),
              ),
              child: Obx(() {
                return Text(
                  scanController.layoutMode.value
                      ? 'Önce paragrafı çekin, ardından soruyu çekin.'
                      : 'Kamerayı yatay tutmayınız',
                  textAlign: TextAlign.center,
                  style: Theme.of(context).textTheme.headlineSmall!.copyWith(
                        color: Colors.white,
                        fontWeight: FontWeight.w700,
                        fontSize: 22,
                      ),
                );
              }),
            ),
          ),
          Align(
            alignment: Alignment.bottomCenter,
            child: Padding(
              padding: EdgeInsets.only(bottom: context.dynamicHeight * 0.01),
              child: Column(
                mainAxisAlignment: MainAxisAlignment.end,
                children: [
                  Row(
                    children: [
                      Expanded(
                        child: GestureDetector(
                          onTap: () {
                            if (scanController.layoutMode.value) {
                              scanController.pickDoubleImageFromGallery(
                                context,
                              );
                            } else {
                              scanController.pickImageFromGallery(
                                context,
                              );
                            }
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
                                scanController.onLayerIconTapped(
                                    context, false);
                              } else {
                                scanController.onLayerIconTapped(context, true);
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
                  SizedBox(
                    height: context.height * 0.03,
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

class OverlayPainter extends CustomPainter {
  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = Colors.white // Çizgi rengi
      ..strokeWidth = 4.0 // Çizgi kalınlığı
      ..style = PaintingStyle.stroke;

    const cornerLength = 40.0; // Köşe çizgilerinin uzunluğu
    const cornerWidth = 4.0; // Çizgi genişliği (Paint ayarıyla aynı)

    // Sol üst köşe
    canvas.drawLine(
      const Offset(0, 0),
      const Offset(cornerLength, 0),
      paint,
    );
    canvas.drawLine(
      const Offset(0, 0),
      const Offset(0, cornerLength),
      paint,
    );

    // Sağ üst köşe
    canvas.drawLine(
      Offset(size.width, 0),
      Offset(size.width - cornerLength, 0),
      paint,
    );
    canvas.drawLine(
      Offset(size.width, 0),
      Offset(size.width, cornerLength),
      paint,
    );

    // Sol alt köşe
    canvas.drawLine(
      Offset(0, size.height),
      Offset(cornerLength, size.height),
      paint,
    );
    canvas.drawLine(
      Offset(0, size.height),
      Offset(0, size.height - cornerLength),
      paint,
    );

    // Sağ alt köşe
    canvas.drawLine(
      Offset(size.width, size.height),
      Offset(size.width - cornerLength, size.height),
      paint,
    );
    canvas.drawLine(
      Offset(size.width, size.height),
      Offset(size.width, size.height - cornerLength),
      paint,
    );

    // Ortada artı (+) işareti
    final center = Offset(size.width / 2, size.height / 2);
    canvas.drawLine(
      Offset(center.dx - 20, center.dy),
      Offset(center.dx + 20, center.dy),
      paint,
    );
    canvas.drawLine(
      Offset(center.dx, center.dy - 20),
      Offset(center.dx, center.dy + 20),
      paint,
    );
  }

  @override
  bool shouldRepaint(CustomPainter oldDelegate) => false;
}
