import NumberEditor from '@components/inputs/NumberEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { UpdateStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';

import { StreamGroupDto, UpdateStreamGroupRequest } from '@lib/smAPI/smapiTypes';
import { useCallback } from 'react';

export interface StreamGroupColumnConfigProps {
  readonly field?: string;
  readonly header?: string;
  readonly width?: number;
}

export const useStreamGroupStartChnColumnConfig = (props?: StreamGroupColumnConfigProps) => {
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
        <div className="flex w-full justify-content-right align-content-center">
          <NumberEditor
            value={StreamGroupDto.StartingChannelNumber}
            onSave={(e) => {
              const StreamGroup = { StartingChannelNumber: e, StreamGroupId: StreamGroupDto.Id } as UpdateStreamGroupRequest;
              update(StreamGroup);
            }}
            tooltip="The starting channel number for the stream group."
          />
        </div>
      );
    },
    [update]
  );

  const columnConfig: ColumnMeta = {
    align: 'right',
    bodyTemplate: bodyTemplate,
    field: 'StartingChannelNumber',
    header: 'Start #',
    width: 30
  };

  return columnConfig;
};
