import { type ColumnFieldType } from './DataSelectorTypes';
import { blankTemplate } from './blankTemplate';
import { defaultTemplate } from './defaultTemplate';
import { epgLinkTemplate } from './epgLinkTemplate';
import { epgSourceTemplate } from './epgSourceTemplate';
import getRecord from './getRecord';
import getRecordString from './getRecordString';
import { imageBodyTemplate } from './imageBodyTemplate';
import { isHiddenTemplate } from './isHiddenTemplate';
import { m3uLinkTemplate } from './m3uLinkTemplate';
import { streamsBodyTemplate } from './streamsBodyTemplate';
import { urlTemplate } from './urlTemplate';

function bodyTemplate(data: object, fieldName: string, fieldType: ColumnFieldType, defaultIcon: string, camelize?: boolean) {
  switch (fieldType) {
    case 'blank': {
      return blankTemplate();
    }
    case 'm3ulink': {
      return m3uLinkTemplate(data as StreamGroupDto);
    }

    case 'epglink': {
      return epgLinkTemplate(data);
    }
    case 'url': {
      return urlTemplate(data);
    }
    case 'epg': {
      return epgSourceTemplate(getRecordString(data, 'user_Tvg_ID'));
    }
    case 'image': {
      return imageBodyTemplate(data, fieldName, defaultIcon);
    }
    case 'streams': {
      const activeCount = getRecord(data, 'activeCount');
      const totalCount = getRecord(data, 'totalCount');

      return streamsBodyTemplate(activeCount, totalCount);
    }
    case 'isHidden': {
      return isHiddenTemplate(data, fieldName);
    }
    default: {
      return defaultTemplate(data, fieldName, camelize);
    }
  }
}

export default bodyTemplate;
