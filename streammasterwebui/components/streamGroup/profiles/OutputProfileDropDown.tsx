import SMDropDown from '@components/sm/SMDropDown';
import useGetOutputProfiles from '@lib/smAPI/Profiles/useGetOutputProfiles';
import { OutputProfileDto } from '@lib/smAPI/smapiTypes';

import { ReactNode, useCallback, useMemo } from 'react';

interface OutputProfileDropDownProps {
  readonly value: string;
  readonly onChange: (value: OutputProfileDto) => void;
  readonly buttonDarkBackground?: boolean;
}

const OutputProfileDropDown = ({ buttonDarkBackground = false, onChange, value }: OutputProfileDropDownProps) => {
  const { data, isLoading } = useGetOutputProfiles();

  const selectedFileProfile = useMemo(() => {
    if (!data) {
      return undefined;
    }

    return data.find((x) => x.ProfileName === value);
  }, [data, value]);

  const itemTemplate = useCallback((option: OutputProfileDto): JSX.Element => {
    return <div className="text-xs text-container">{option?.ProfileName ?? ''}</div>;
  }, []);

  const buttonTemplate = useMemo((): ReactNode => {
    return (
      <div className="sm-epg-selector">
        <div className="text-container" style={{ paddingLeft: '0.12rem' }}>
          {selectedFileProfile?.ProfileName ?? ''}
        </div>
      </div>
    );
  }, [selectedFileProfile]);

  return (
    <SMDropDown
      isOverLayLoading={isLoading}
      buttonTemplate={buttonTemplate}
      buttonDarkBackground={buttonDarkBackground}
      contentWidthSize="2"
      data={data}
      dataKey="label"
      info=""
      itemTemplate={itemTemplate}
      label={buttonDarkBackground ? 'File Profile' : undefined}
      onChange={onChange}
      optionValue="label"
      scrollHeight="20vh"
      title="Profiles"
      value={selectedFileProfile}
    />
  );
};

export default OutputProfileDropDown;
