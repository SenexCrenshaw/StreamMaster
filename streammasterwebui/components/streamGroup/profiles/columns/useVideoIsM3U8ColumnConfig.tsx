import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { UpdateCommandProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { CommandProfileDto, UpdateCommandProfileRequest } from '@lib/smAPI/smapiTypes';
import { Checkbox } from 'primereact/checkbox';
import { useCallback } from 'react';
import { OutputProfileColumnConfigProps } from './useOutputProfileColumnConfig';

export const useVideoIsM3U8ColumnConfig = (props?: OutputProfileColumnConfigProps) => {
  const update = useCallback((request: UpdateCommandProfileRequest) => {
    UpdateCommandProfile(request)
      .then((res) => {})
      .catch((error) => {
        console.log('error', error);
      })
      .finally();
  }, []);

  const bodyTemplate = useCallback(
    (videoOutputProfileDto: CommandProfileDto) => {
      return (
        <div className="flex w-full justify-content-center align-content-center">
          <Checkbox
            checked={videoOutputProfileDto.IsM3U8}
            onChange={(e) => {
              const outputProfile = { IsM3U8: e.checked, ProfileName: videoOutputProfileDto.ProfileName } as UpdateCommandProfileRequest;
              update(outputProfile);
            }}
          />
        </div>
      );
    },
    [update]
  );

  const columnConfig: ColumnMeta = {
    align: 'left',
    bodyTemplate: bodyTemplate,
    field: 'IsM3U8',
    header: 'Is M3U8',
    width: 30
  };

  return columnConfig;
};
