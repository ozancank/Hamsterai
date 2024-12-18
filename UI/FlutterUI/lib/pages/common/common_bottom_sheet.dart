import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:flutter_svg/flutter_svg.dart';
import 'package:get/get.dart';
import 'package:mobile/core/constants/assets_constant.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/pages/auth/controller/login_controller.dart';
import 'package:mobile/pages/common/common_button.dart';
import 'package:mobile/styles/colors.dart';
import 'package:mobile/utils/device_helper.dart';

class CommonBottomSheet extends StatefulWidget {
  final void Function()? approveOnTap;
  final void Function()? cancelOnTap;
  final String headerText;
  final String smallText;
  final String cancelText;
  final String approveText;
  final Color? approveButtonColor;
  final Color? cancelButtonColor;
  final Color? cancelTextColor;
  final bool isScan;
  final List<int>? resizedBytes;
  const CommonBottomSheet({
    super.key,
    this.approveOnTap,
    this.cancelOnTap,
    this.headerText = 'Emin misiniz?',
    this.smallText = 'Kullanıcı çıkışı yapmaktasınız.',
    this.cancelText = 'İptal',
    this.approveText = 'Çıkış Yap',
    this.cancelButtonColor,
    this.cancelTextColor,
    this.approveButtonColor,
    this.isScan = false,
    this.resizedBytes,
  });

  @override
  State<CommonBottomSheet> createState() => _CommonBottomSheetState();
}

class _CommonBottomSheetState extends State<CommonBottomSheet> {
  final controller = Get.put(LoginController());
  @override
  Widget build(BuildContext context) {
    return FractionallySizedBox(
      heightFactor: isTablet(context)
          ? 0.30
          : widget.resizedBytes != null
              ? 0.70
              : 0.35,
      child: Padding(
        padding: const EdgeInsets.symmetric(horizontal: 24.0, vertical: 10.0),
        child: SingleChildScrollView(
          child: Column(
            children: [
              widget.resizedBytes != null
                  ? Image.memory(
                      Uint8List.fromList(widget.resizedBytes!),
                      width: double.infinity,
                      height: context.dynamicHeight * 0.2,
                      fit: BoxFit.contain,
                    )
                  : const SizedBox(),
              const SizedBox(height: 16),
              SizedBox(height: context.height * 0.04),
              Text(
                widget.headerText,
                style: Theme.of(context).textTheme.headlineSmall!.copyWith(
                      color: MyColors.darkBlue,
                      fontWeight: FontWeight.w600,
                    ),
                textAlign: TextAlign.center,
              ),
              Text(
                widget.smallText,
                textAlign: TextAlign.center,
                style: Theme.of(context).textTheme.bodyMedium!.copyWith(
                      color: MyColors.darkBlue,
                      fontWeight: FontWeight.w400,
                    ),
              ),
              CommonButton(
                text: widget.approveText,
                textColor: Colors.white,
                bgcolor: widget.approveButtonColor ?? MyColors.primaryColor,
                bottomSheet: true,
                onTap: widget.approveOnTap ??
                    () {
                      HapticFeedback.mediumImpact();
                      controller.logout(context);
                    },
              ),
              CommonButton(
                bottomSheet: true,
                text: widget.cancelText,
                textColor: widget.cancelTextColor ?? Colors.white,
                bgcolor: widget.cancelButtonColor ?? const Color(0xFFDD5050),
                onTap: widget.cancelOnTap ??
                    () {
                      HapticFeedback.mediumImpact();
                      Get.back();
                    },
              ),
            ],
          ),
        ),
      ),
    );
  }
}
