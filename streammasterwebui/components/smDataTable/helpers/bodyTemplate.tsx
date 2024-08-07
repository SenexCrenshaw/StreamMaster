import { StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { epgLinkTemplate } from '../hooks/epgLinkTemplate';
import { m3uLinkTemplate } from '../hooks/m3uLinkTemplate';
import { urlTemplate } from '../hooks/urlTemplate';
import { blankTemplate } from '../templates/blankTemplate';
import { defaultTemplate } from '../templates/defaultTemplate';
import { epgSourceTemplate } from '../templates/epgSourceTemplate';
import { imageBodyTemplate } from '../templates/imageBodyTemplate';
import { isHiddenTemplate } from '../templates/isHiddenTemplate';
import { streamsBodyTemplate } from '../templates/streamsBodyTemplate';
import { ColumnFieldType } from '../types/smDataTableTypes';
import getRecord from './getRecord';
import getRecordString from './getRecordString';

function bodyTemplate(data: object, fieldName: string, fieldType: ColumnFieldType, defaultIcon: string, camelize?: boolean, dataKey?: string) {
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
      const activeCount = getRecord({ data, dataKey, fieldName: 'activeCount' });
      const totalCount = getRecord({ data, dataKey, fieldName: 'totalCount' });

      return streamsBodyTemplate(activeCount, totalCount);
    }
    case 'isHidden': {
      return isHiddenTemplate(data, fieldName);
    }
    default: {
      return defaultTemplate(data, fieldName, camelize, dataKey);
    }
  }
}

export default bodyTemplate;
