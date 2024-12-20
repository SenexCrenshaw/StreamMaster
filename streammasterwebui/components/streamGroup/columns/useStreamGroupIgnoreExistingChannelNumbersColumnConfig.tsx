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

export const useStreamGroupIgnoreExistingChannelNumbersColumnConfig = (props?: StreamGroupColumnConfigProps) => {
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
            checked={StreamGroupDto?.IgnoreExistingChannelNumbers}
            onChange={(e) => {
              const StreamGroup = { IgnoreExistingChannelNumbers: e, StreamGroupId: StreamGroupDto.Id } as UpdateStreamGroupRequest;
              update(StreamGroup);
            }}
            tooltip="Overwrite existing channel numbers when assigning channel numbers to streams."
          />
        </div>
      );
    },
    [update]
  );

  const columnConfig: ColumnMeta = {
    align: 'right',
    bodyTemplate: bodyTemplate,
    field: 'IgnoreExistingChannelNumbers',
    header: 'Overwrite #s',
    width: 44
  };

  return columnConfig;
};
