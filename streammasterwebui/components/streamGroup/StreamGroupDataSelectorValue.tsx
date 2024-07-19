import StringEditor from '@components/inputs/StringEditor';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { Logger } from '@lib/common/logger';
import { UpdateStreamGroupProfile } from '@lib/smAPI/StreamGroups/StreamGroupsCommands';
import { StreamGroupDto, StreamGroupProfile, UpdateStreamGroupProfileRequest } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useMemo } from 'react';
import AttachStreamGroupProfileDialog from './profiles/AttachStreamGroupProfileDialog';
import CommandProfileDropDown from './profiles/CommandProfileDropDown';
import OutputProfileDropDown from './profiles/OutputProfileDropDown';
import RemoveStreamGroupProfileDialog from './profiles/RemoveStreamGroupProfileDialog';
interface StreamGroupDataSelectorValueProperties {
  readonly id: string;
  readonly streamGroupDto: StreamGroupDto;
}
const StreamGroupDataSelectorValue = ({ id, streamGroupDto }: StreamGroupDataSelectorValueProperties) => {
  const dataKey = `${id}-StreamGroupDataSelectorValue`;
  Logger.debug('StreamGroupDataSelectorValue ', streamGroupDto.StreamGroupProfiles);
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
    (streamGroupProfile: StreamGroupProfile) => <RemoveStreamGroupProfileDialog streamGroupProfile={streamGroupProfile} />,
    []
  );

  const nameTemplate = useCallback(
    (rowData: StreamGroupDto) => {
      if (rowData.IsReadOnly === true || rowData.Name.toLowerCase() === 'default') {
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
      var found = streamGroupDto.StreamGroupProfiles.find((x) => x.Id === rowData.Id);
      if (found === undefined) {
        return <div />;
      }
      return (
        <OutputProfileDropDown
          value={found.OutputProfileName}
          onChange={(e) => {
            if (e !== undefined) {
              const ret = { Name: rowData.Name, OutputProfileName: e.ProfileName } as UpdateStreamGroupProfileRequest;
              update(ret);
            }
          }}
        />
      );
    },
    [streamGroupDto.StreamGroupProfiles, update]
  );

  const CommandProfileTemplate = useCallback(
    (rowData: StreamGroupProfile) => {
      var found = streamGroupDto.StreamGroupProfiles.find((x) => x.Id === rowData.Id);
      if (found === undefined) {
        return <div />;
      }

      return (
        <CommandProfileDropDown
          value={found.CommandProfileName}
          onChange={(e) => {
            if (e !== undefined) {
              const ret = { Name: rowData.Name, CommandProfileName: e.ProfileName } as UpdateStreamGroupProfileRequest;
              update(ret);
            }
          }}
        />
      );
    },
    [streamGroupDto.StreamGroupProfiles, update]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      { bodyTemplate: nameTemplate, field: 'Name', sortable: false, width: 80 },
      { bodyTemplate: fileProfileTemplate, field: 'OutputProfileName', header: 'Output', sortable: false, width: 50 },
      { bodyTemplate: CommandProfileTemplate, field: 'CommandProfileName', header: 'Video', sortable: false, width: 50 },
      {
        align: 'center',
        field: 'HDHRLink',
        fieldType: 'url',
        width: 16
      },
      {
        field: 'epglink',
        fieldType: 'epglink',
        width: 16
      },
      {
        field: 'm3ulink',
        fieldType: 'm3ulink',
        width: 16
      },
      {
        align: 'center',
        bodyTemplate: actionTemplate,
        field: '',
        fieldType: 'actions',
        header: 'Actions',
        width: 12
      }
    ],
    [actionTemplate, fileProfileTemplate, nameTemplate, CommandProfileTemplate]
  );

  if (!streamGroupDto?.StreamGroupProfiles) {
    return null;
  }

  return (
    <div onClick={() => {}}>
      <SMDataTable
        actionHeaderTemplate={<AttachStreamGroupProfileDialog streamGroupDto={streamGroupDto} />}
        columns={columns}
        dataSource={streamGroupDto.StreamGroupProfiles}
        defaultSortField="Name"
        defaultSortOrder={1}
        emptyMessage="No Profiles"
        enablePaginator={false}
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
