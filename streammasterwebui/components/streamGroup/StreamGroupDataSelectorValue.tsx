import StringEditor from '@components/inputs/StringEditor';

import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { Logger } from '@lib/common/logger';
import { UpdateStreamGroupProfile } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';

import { StreamGroupDto, StreamGroupProfile, UpdateStreamGroupProfileRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo } from 'react';
import CreateStreamGroupProfileDialog from './profiles/CreateStreamGroupProfileDialog';
import FileProfileDropDown from './profiles/FileProfileDropDown';
import RemoveStreamGroupProfileDialog from './profiles/RemoveStreamGroupProfileDialog';
import VideoProfileDropDown from './profiles/VideoProfileDropDown';

interface StreamGroupDataSelectorValueProperties {
  readonly id: string;
  readonly streamGroupDto: StreamGroupDto;
}

const StreamGroupDataSelectorValue = ({ id, streamGroupDto }: StreamGroupDataSelectorValueProperties) => {
  const dataKey = `${id}-StreamGroupDataSelectorValue`;

  const update = useCallback(
    (request: UpdateStreamGroupProfileRequest) => {
      Logger.debug('UpdateStreamGroupProfileRequest', request);
      request.StreamGroupId = streamGroupDto.Id;
      UpdateStreamGroupProfile(request)
        .then((res) => {})
        .catch((error) => {
          console.error('error', error);
        })
        .finally();
    },
    [streamGroupDto.Id]
  );

  const actionTemplate = useCallback(
    (streamGroupProfile: StreamGroupProfile) => (
      <div className="flex align-content-center justify-content-end">
        <RemoveStreamGroupProfileDialog streamGroupProfile={streamGroupProfile} />
      </div>
    ),
    []
  );

  const nameTemplate = useCallback(
    (rowData: StreamGroupDto) => {
      if (rowData.IsReadOnly === true) {
        return <div className="text-container pl-1">{rowData.Name}</div>;
      }
      return (
        <StringEditor
          value={rowData.Name}
          onSave={(e) => {
            if (e !== undefined) {
              const ret = { Name: rowData.Name, NewName: e } as UpdateStreamGroupProfileRequest;
              update(ret);
            }
          }}
        />
      );
    },
    [update]
  );

  const fileProfileTemplate = useCallback(
    (rowData: StreamGroupProfile) => {
      return (
        <FileProfileDropDown
          value={rowData.FileProfileName}
          onChange={(e) => {
            if (e !== undefined) {
              const ret = { FileProfileName: e.Name, Name: rowData.Name } as UpdateStreamGroupProfileRequest;
              update(ret);
            }
          }}
        />
      );
    },
    [update]
  );

  const videoProfileTemplate = useCallback(
    (rowData: StreamGroupProfile) => {
      return (
        <VideoProfileDropDown
          value={rowData.VideoProfileName}
          onChange={(e) => {
            if (e !== undefined) {
              const ret = { Name: rowData.Name, VideoProfileName: e.Name } as UpdateStreamGroupProfileRequest;
              update(ret);
            }
          }}
        />
      );
    },
    [update]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      { bodyTemplate: nameTemplate, field: 'Name', sortable: false, width: '4rem' },
      { bodyTemplate: fileProfileTemplate, field: 'FileProfileName', sortable: false, width: '4rem' },
      { bodyTemplate: videoProfileTemplate, field: 'VideoProfileName', sortable: false, width: '4rem' },
      {
        align: 'right',
        bodyTemplate: actionTemplate,
        field: '',
        fieldType: 'actions',
        header: 'Actions',
        width: '2rem'
      }
    ],
    [actionTemplate, fileProfileTemplate, nameTemplate, videoProfileTemplate]
  );

  const actionHeaderTemplate = useMemo(() => {
    return (
      <div className="flex justify-content-around">
        Actions
        <CreateStreamGroupProfileDialog streamGroupDto={streamGroupDto} />
      </div>
    );
  }, [streamGroupDto]);

  if (!streamGroupDto?.StreamGroupProfiles) {
    return null;
  }

  return (
    <div onClick={() => {}}>
      <SMDataTable
        actionHeaderTemplate={actionHeaderTemplate}
        columns={columns}
        dataSource={streamGroupDto.StreamGroupProfiles}
        defaultSortField="Name"
        defaultSortOrder={1}
        emptyMessage="No Profiles"
        enablePaginator
        headerSize="small"
        id={dataKey}
        noSourceHeader
        rows={5}
        selectedItemsKey={'StreamGroupDataSelectorValue-selectSelectedSMStreamDtoItems'}
      />
    </div>
  );
};

export default memo(StreamGroupDataSelectorValue);
