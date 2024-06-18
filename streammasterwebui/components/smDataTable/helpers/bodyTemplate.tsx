import { StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { blankTemplate } from '../../dataSelector/blankTemplate';
import { defaultTemplate } from '../../dataSelector/defaultTemplate';
import { epgSourceTemplate } from '../../dataSelector/epgSourceTemplate';
import getRecordString from '../../dataSelector/getRecordString';
import { imageBodyTemplate } from '../../dataSelector/imageBodyTemplate';
import { isHiddenTemplate } from '../../dataSelector/isHiddenTemplate';
import { streamsBodyTemplate } from '../../dataSelector/streamsBodyTemplate';
import { epgLinkTemplate } from '../hooks/epgLinkTemplate';
import { m3uLinkTemplate } from '../hooks/m3uLinkTemplate';
import { urlTemplate } from '../hooks/urlTemplate';
import { ColumnFieldType } from '../types/smDataTableTypes';
import getRecord from './getRecord';

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
