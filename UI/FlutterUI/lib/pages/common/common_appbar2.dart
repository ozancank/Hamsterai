import 'package:flutter/material.dart';
import 'package:flutter/services.dart';
import 'package:get/get.dart';
import 'package:mobile/core/extensions/size_extension.dart';
import 'package:mobile/styles/colors.dart';

class CommonAppbar2 extends StatelessWidget implements PreferredSizeWidget {
  const CommonAppbar2({
    super.key,
    required this.title,
    this.isMyQuestion = false,
    this.onTap,
    this.subTitle = '',
    this.isDetail = false,
    this.subTitleWidget,
    this.widget = const SizedBox(),
  });
  final String title;
  final bool isMyQuestion;
  final String subTitle;
  final VoidCallback? onTap;
  final bool isDetail;
  final Widget widget;
  final Widget? subTitleWidget;
  @override
  Widget build(BuildContext context) {
    return ClipRRect(
      borderRadius: const BorderRadius.only(
        bottomLeft: Radius.circular(30),
        bottomRight: Radius.circular(30),
      ),
      child: AppBar(
        centerTitle: false,
        backgroundColor: MyColors.primaryColor,
        toolbarHeight: context.dynamicHeight * 0.1,
        leading: InkWell(
          onTap: () {
            HapticFeedback.mediumImpact();
            Get.back();
          },
          child: Container(
            padding: const EdgeInsets.all(20),
            child: const Icon(
              Icons.arrow_back_rounded,
              color: Colors.white,
              size: 25,
            ),
            // child: SvgPicture.asset(
            //   AssetsConstant.appbarBack,
            //   height: 5,
            // ),
          ),
        ),
        title: Row(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  title,
                  style: Theme.of(context).textTheme.headlineSmall!.copyWith(
                      color: Colors.white,
                      fontFamily: 'Quicksand',
                      fontWeight: FontWeight.w600),
                ),
                if (subTitle != '')
                  Text(
                    subTitle.length > 20
                        ? '${subTitle.substring(0, 20)}...'
                        : subTitle,
                    overflow: TextOverflow.ellipsis,
                    style: Theme.of(context).textTheme.bodyMedium!.copyWith(
                        color: Colors.white,
                        fontFamily: 'Quicksand',
                        overflow: TextOverflow.ellipsis,
                        fontWeight: FontWeight.w600),
                  ),
                if (subTitleWidget != null) subTitleWidget!
              ],
            ),
          ],
        ),
        actions: [
          if (isMyQuestion)
            IconButton(
              onPressed: onTap,
              icon: const Icon(
                Icons.filter_alt,
                color: Colors.white,
              ),
            ),
          if (isDetail) widget
        ],
      ),
    );
  }

  @override
  Size get preferredSize => const Size.fromHeight(80);
}
