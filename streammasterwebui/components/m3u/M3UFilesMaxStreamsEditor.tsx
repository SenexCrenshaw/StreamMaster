import NumberEditor from '@components/inputs/NumberEditor';
import { getTopToolOptions } from '@lib/common/common';
import { isDev } from '@lib/settings';
import { UpdateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { M3UFileDto, UpdateM3UFileRequest } from '@lib/smAPI/smapiTypes';
import { CSSProperties, memo, useCallback } from 'react';

interface M3UFilesMaxStreamsEditorProperties {
  readonly data: M3UFileDto;
  readonly style?: CSSProperties;
}

const M3UFilesMaxStreamsEditor = ({ data, style }: M3UFilesMaxStreamsEditorProperties) => {
  const onUpdateM3UFile = useCallback(
    async (maxStreamCount: number) => {
      if (data.Id === 0 || data.MaxStreamCount === maxStreamCount) {
        return;
      }

      const toSend = {} as UpdateM3UFileRequest;

      toSend.Id = data.Id;
      toSend.MaxStreamCount = maxStreamCount;

      await UpdateM3UFile(toSend)
        .then(() => {})
        .catch((error) => {
          console.log(error);
        });
    },
    [data.Id, data.MaxStreamCount]
  );

  return (
    <NumberEditor
      darkBackGround
      onChange={async (e) => {
        await onUpdateM3UFile(e);
      }}
      showSave={false}
      tooltip={isDev ? `id: ${data.Id}` : undefined}
      tooltipOptions={getTopToolOptions}
      value={data.MaxStreamCount}
    />
  );
};

M3UFilesMaxStreamsEditor.displayName = 'Channel Number Editor';

export default memo(M3UFilesMaxStreamsEditor);
