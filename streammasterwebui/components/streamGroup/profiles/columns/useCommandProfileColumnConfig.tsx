import StringEditor from '@components/inputs/StringEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { UpdateCommandProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { CommandProfileDto, UpdateCommandProfileRequest } from '@lib/smAPI/smapiTypes';
import { useCallback } from 'react';

export interface CommandProfileColumnConfigProps {
  readonly field?: string;
  readonly header?: string;
  readonly width?: number | string;
}

interface IntCommandProfileColumnConfigProps {
  readonly field: string;
  readonly header: string;
  readonly width?: number | string;
}

export const useCommandProfileColumnConfig = ({ field, header, width = 80 }: IntCommandProfileColumnConfigProps) => {
  function updateProfile<T extends keyof UpdateCommandProfileRequest>(profile: UpdateCommandProfileRequest, key: T, value: UpdateCommandProfileRequest[T]) {
    if (value !== undefined && value !== null) {
      profile[key] = value;
    }
  }

  const update = useCallback((request: UpdateCommandProfileRequest) => {
    console.log('update', request);

    UpdateCommandProfile(request)
      .then((res) => {})
      .catch((error) => {
        console.log('error', error);
      })
      .finally();
  }, []);

  const bodyTemplate = useCallback(
    (videoOutputProfile: CommandProfileDto) => {
      var key = field as keyof CommandProfileDto;
      let value = videoOutputProfile[key] as string;

      if (videoOutputProfile.ProfileName === 'StreamMaster') {
        return (
          <div className="sm-epg-selector">
            <div className="text-container pl-1"></div>
          </div>
        );
      }
      if (videoOutputProfile.IsReadOnly === true) {
        return (
          <div className="sm-epg-selector">
            <div className="text-container pl-1">{value}</div>
          </div>
        );
      }

      return (
        <StringEditor
          onSave={(e) => {
            if (e !== undefined) {
              const UpdateCommandProfileRequest = { ProfileName: videoOutputProfile.ProfileName } as UpdateCommandProfileRequest;
              var key = field as keyof UpdateCommandProfileRequest;
              updateProfile(UpdateCommandProfileRequest, key, e);

              update(UpdateCommandProfileRequest);
            }
          }}
          value={value}
        />
      );
    },
    [field, update]
  );

  const columnConfig: ColumnMeta = {
    bodyTemplate: bodyTemplate,
    field: field,
    header: header,
    width: width
  };

  return columnConfig;
};
