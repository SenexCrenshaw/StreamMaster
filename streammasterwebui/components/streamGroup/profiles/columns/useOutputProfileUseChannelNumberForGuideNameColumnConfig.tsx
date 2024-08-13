import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { UpdateOutputProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { OutputProfileDto, UpdateOutputProfileRequest } from '@lib/smAPI/smapiTypes';
import { Checkbox } from 'primereact/checkbox';
import { useCallback } from 'react';
import { OutputProfileColumnConfigProps } from './useOutputProfileColumnConfig';

export const useOutputProfileUseChannelNumberForGuideNameColumnConfig = (props?: OutputProfileColumnConfigProps) => {
  const update = useCallback((request: UpdateOutputProfileRequest) => {
    UpdateOutputProfile(request)
      .then((res) => {})
      .catch((error) => {
        console.log('error', error);
      })
      .finally();
  }, []);

  const bodyTemplate = useCallback(
    (outputProfileDto: OutputProfileDto) => {
      return (
        <div className="flex w-full justify-content-center align-content-center">
          <Checkbox
            checked={outputProfileDto.AppendChannelNumberToId}
            onChange={(e) => {
              const outputProfile = { ProfileName: outputProfileDto.ProfileName, AppendChannelNumberToId: e.checked } as UpdateOutputProfileRequest;
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
    field: 'AppendChannelNumberToId',
    header: 'Use Ch# Name',
    width: 40
  };

  return columnConfig;
};
