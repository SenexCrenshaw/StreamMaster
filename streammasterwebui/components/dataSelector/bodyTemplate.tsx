import { camel2title } from '@lib/common/common';
import { StreamGroupDto } from '@lib/iptvApi';
import { LinkButton } from '../buttons/LinkButton';
import { type ColumnFieldType } from './DataSelectorTypes';
import getRecord from './getRecord';
import getRecordString from './getRecordString';

function epgSourceTemplate(tvgid: string) {
  return (
    <div>
      <div className="flex justify-content-start">{tvgid}</div>
    </div>
  );
}

function imageBodyTemplate(data: object, fieldName: string, defaultIcon: string) {
  const record = getRecordString(data, fieldName);

  return (
    <div className="iconselector flex align-contents-center w-full min-w-full">
      <img
        alt={record ?? 'Logo'}
        className="max-h-1rem max-w-full p-0"
        src={`${encodeURI(record ?? '')}`}
        style={{
          objectFit: 'contain'
        }}
        loading="lazy"
      />
    </div>
  );
}

function streamsBodyTemplate(activeCount: string, totalCount: string) {
  if (activeCount === null || totalCount === undefined) {
    return null;
  }

  return (
    <div className="flex align-items-center gap-2">
      {activeCount}/{totalCount}
    </div>
  );
}

function m3uLinkTemplate(data: StreamGroupDto) {
  return (
    <div className="flex justify-content-center align-items-center gap-1">
      <LinkButton filled link={getRecordString(data, 'm3ULink')} title="M3U Link" />
      <LinkButton bolt link={getRecordString(data, 'shortM3ULink')} title="Unsecure M3U Link" />
    </div>
  );
}

function epgLinkTemplate(data: object) {
  return (
    <div className="flex justify-content-center align-items-center gap-1">
      <LinkButton filled link={getRecordString(data, 'xmlLink')} title="EPG Link" />
      <LinkButton bolt link={getRecordString(data, 'shortEPGLink')} title="Unsecure EPG Link" />
    </div>
  );
}

function urlTemplate(data: object) {
  return <LinkButton filled link={getRecordString(data, 'hdhrLink')} title="HDHR Link" />;
}

function blankTemplate() {
  return <div />;
}

function isHiddenTemplate(data: object, fieldName: string) {
  const record = getRecord(data, fieldName);

  return record ? <i className="pi pi-eye-slash text-blue-500" /> : <i className="pi pi-eye text-green-500" />;
}

function defaultTemplate(data: object, fieldName: string, camelize?: boolean) {
  let recordJson = JSON.stringify(getRecord(data, fieldName));

  if (!recordJson) {
    console.error('recordJson is null', data, fieldName);
    recordJson = '';
  }
  if (recordJson.startsWith('"') && recordJson.endsWith('"')) {
    recordJson = recordJson.substring(1, recordJson.length - 1);
  }

  if (camelize) {
    recordJson = camel2title(recordJson);
  }

  return (
    <span
      style={{
        display: 'block',
        overflow: 'hidden',
        padding: '0rem !important',
        textOverflow: 'ellipsis',
        whiteSpace: 'nowrap'
      }}
    >
      {recordJson}
    </span>
  );
}

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
