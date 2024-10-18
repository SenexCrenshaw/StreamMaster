import BooleanEditor from '@components/inputs/BooleanEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { UpdateStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';

import { StreamGroupDto, UpdateStreamGroupRequest } from '@lib/smAPI/smapiTypes';
import { useCallback } from 'react';

export interface StreamGroupColumnConfigProps {
  readonly field?: string;
  readonly header?: string;
  readonly width?: number;
}

export const useStreamGroupAutoSetChannelNumbersColumnConfig = (props?: StreamGroupColumnConfigProps) => {
  const update = useCallback((request: UpdateStreamGroupRequest) => {
    UpdateStreamGroup(request)
      .then((res) => {})
      .catch((error) => {
        console.log('error', error);
      })
      .finally();
  }, []);

  const bodyTemplate = useCallback(
    (StreamGroupDto: StreamGroupDto) => {
      return (
        <div className="flex w-full justify-content-center align-content-center">
          <BooleanEditor
            checked={StreamGroupDto?.AutoSetChannelNumbers}
            onChange={(e) => {
              const StreamGroup = { AutoSetChannelNumbers: e, StreamGroupId: StreamGroupDto.Id } as UpdateStreamGroupRequest;
              update(StreamGroup);
            }}
            tooltip="Automatically assign channel numbers to streams."
          />
        </div>
      );
    },
    [update]
  );

  const columnConfig: ColumnMeta = {
    align: 'right',
    bodyTemplate: bodyTemplate,
    field: 'AutoSetChannelNumbers',
    header: 'Auto Set',
    width: 30
  };

  return columnConfig;
};
