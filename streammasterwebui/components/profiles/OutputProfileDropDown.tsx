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
      <div className="text-container" style={{ paddingLeft: '0.24rem' }}>
        {selectedFileProfile?.ProfileName ?? ''}
      </div>
    );
  }, [selectedFileProfile]);

  return (
    <SMDropDown
      buttonDarkBackground={buttonDarkBackground}
      buttonTemplate={buttonTemplate}
      contentWidthSize="2"
      data={data}
      dataKey="ProfileName"
      info=""
      isOverLayLoading={isLoading}
      itemTemplate={itemTemplate}
      label={buttonDarkBackground ? 'Output Profile' : undefined}
      onChange={onChange}
      propertyToMatch="ProfileName"
      scrollHeight="20vh"
      title="Profiles"
      value={value}
      zIndex={12}
    />
  );
};

export default OutputProfileDropDown;
