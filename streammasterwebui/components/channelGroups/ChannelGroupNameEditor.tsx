import StringEditor from '@components/inputs/StringEditor';
import { UpdateChannelGroup } from '@lib/smAPI/ChannelGroups/ChannelGroupsCommands';
import { ChannelGroupDto, UpdateChannelGroupRequest } from '@lib/smAPI/smapiTypes';
import React from 'react';

export interface ChannelGroupNameEditorProperties {
  readonly data: ChannelGroupDto;
  readonly onClick?: () => void;
}

const ChannelGroupNameEditor = ({ data, onClick }: ChannelGroupNameEditorProperties) => {
  const onUpdateChannelGroup = React.useCallback(
    async (name: string) => {
      if (data.Id === 0 || !name || name === '' || data.Name === name) {
        return;
      }

      const toSend = { ChannelGroupId: data.Id, NewGroupName: name } as UpdateChannelGroupRequest;

      await UpdateChannelGroup(toSend)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        });
    },
    [data.Id, data.Name]
  );

  if (data.Name === undefined) {
    return <span className="sm-inputtext" />;
  }

  if (data.IsReadOnly) {
    return <span className="sm-inputtext">{data.Name}</span>;
  }

  return (
    <StringEditor
      showClear
      disabled={data.IsReadOnly}
      darkBackGround={false}
      onClick={onClick}
      onSave={async (e) => {
        if (e !== undefined) {
          await onUpdateChannelGroup(e);
        }
      }}
      value={data.Name}
    />
  );
};

ChannelGroupNameEditor.displayName = 'Channel Number Editor';

export default React.memo(ChannelGroupNameEditor);
