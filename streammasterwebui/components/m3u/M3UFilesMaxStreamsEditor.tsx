import NumberEditorBodyTemplate from '@components/inputs/NumberEditorBodyTemplate';
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
      if (data.id === 0 || data.maxStreamCount === maxStreamCount) {
        return;
      }

      const toSend = {} as UpdateM3UFileRequest;

      toSend.id = data.id;
      toSend.maxStreamCount = maxStreamCount;

      await UpdateM3UFile(toSend)
        .then(() => {})
        .catch((error) => {
          console.log(error);
        });
    },
    [data.id, data.maxStreamCount]
  );

  return (
    <NumberEditorBodyTemplate
      onChange={async (e) => {
        await onUpdateM3UFile(e);
      }}
      showSave={false}
      tooltip={isDev ? `id: ${data.id}` : undefined}
      tooltipOptions={getTopToolOptions}
      value={data.maxStreamCount}
    />
  );
};

M3UFilesMaxStreamsEditor.displayName = 'Channel Number Editor';

export default memo(M3UFilesMaxStreamsEditor);
