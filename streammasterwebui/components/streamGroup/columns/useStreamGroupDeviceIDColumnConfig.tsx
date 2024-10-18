import StringEditor from '@components/inputs/StringEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { UpdateStreamGroup } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';

import { StreamGroupDto, UpdateStreamGroupRequest } from '@lib/smAPI/smapiTypes';
import { useCallback } from 'react';

export interface StreamGroupColumnConfigProps {
  readonly field?: string;
  readonly header?: string;
  readonly width?: number;
}

export const useStreamGroupDeviceIDColumnConfig = (props?: StreamGroupColumnConfigProps) => {
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
          <StringEditor
            value={StreamGroupDto.DeviceID}
            onSave={(e) => {
              const StreamGroup = { DeviceID: e, StreamGroupId: StreamGroupDto.Id } as UpdateStreamGroupRequest;
              update(StreamGroup);
            }}
            tooltip="HDHR Device ID."
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
    header: 'Device ID',
    width: 46
  };

  return columnConfig;
};
