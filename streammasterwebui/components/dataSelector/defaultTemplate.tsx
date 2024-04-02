import { camel2title } from '@lib/common/common';
import getRecord from '../smDataTable/helpers/getRecord';

export function defaultTemplate(data: object, fieldName: string, camelize?: boolean) {
  let recordJson = JSON.stringify(getRecord(data, fieldName));

  if (!recordJson) {
    // console.error('recordJson is null', data, fieldName);
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
