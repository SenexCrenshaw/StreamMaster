import SearchButton from '@components/buttons/SearchButton';
import TextInput from '@components/inputs/TextInput';
import { Countries, SettingDto, useSchedulesDirectGetCountriesQuery } from '@lib/iptvApi';
import { useSelectedCountry } from '@lib/redux/slices/selectedCountrySlice';
import { useSelectedPostalCode } from '@lib/redux/slices/selectedPostalCodeSlice';
import { UpdateSetting } from '@lib/smAPI/Settings/SettingsMutateAPI';
import { Dropdown } from 'primereact/dropdown';
import React, { useState } from 'react';

interface SchedulesDirectCountrySelectorProperties {
  readonly onChange?: (value: string) => void;
  // readonly value?: string | null;
}

const SchedulesDirectCountrySelector = (props: SchedulesDirectCountrySelectorProperties) => {
  const { selectedCountry, setSelectedCountry } = useSelectedCountry('Country');
  const { selectedPostalCode, setSelectedPostalCode } = useSelectedPostalCode('ZipCode');

  const [originalCountry, setOriginalCountry] = useState<string | undefined>();
  const [originalPostalCode, setOriginalPostalCode] = useState<string | undefined>();

  const getCountriesQuery = useSchedulesDirectGetCountriesQuery();

  React.useEffect(() => {
    if (selectedCountry !== undefined && selectedCountry !== originalCountry) {
      setOriginalCountry(selectedCountry);
    }
  }, [selectedCountry, setOriginalCountry]);

  React.useEffect(() => {
    if (selectedPostalCode !== undefined && selectedPostalCode !== originalPostalCode) {
      setOriginalPostalCode(selectedPostalCode);
    }
  }, [selectedPostalCode, setOriginalPostalCode]);

  interface Country {
    shortName: string;
    fullName?: string;
  }

  interface CountryOption {
    label: string;
    value: string;
  }

  const options: CountryOption[] = React.useMemo(() => {
    if (!getCountriesQuery.data) return [];

    const countries: CountryOption[] = [];

    Object.values(getCountriesQuery.data as Countries).forEach((continentCountries) => {
      continentCountries
        .filter((c): c is Country => c.shortName !== undefined && c.shortName.trim() !== '')
        .forEach((c) => {
          countries.push({
            label: c.fullName || 'Unknown Country',
            value: c.shortName ?? ''
          });
        });
    });

    return countries.sort((a, b) => a.label.localeCompare(b.label));
  }, [getCountriesQuery.data]);

  return (
    <div className="flex grid col-12 pl-1 justify-content-start align-items-center p-0 m-0 w-full">
      <div className="flex col-2 p-0 pr-2">
        <Dropdown
          className="bordered-text w-full"
          filter
          onChange={(e) => {
            setSelectedCountry(e.value);
            props.onChange?.(e.value);
          }}
          options={options}
          placeholder="Country"
          style={{
            backgroundColor: 'var(--mask-bg)',
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap'
          }}
          value={selectedCountry}
        />
      </div>
      <div className="flex col-2 p-0">
        <div className="flex col-10 pt-2 p-0 w-full">
          <TextInput
            placeHolder="Postal Code"
            onChange={(e) => {
              setOriginalPostalCode(e);
            }}
            value={originalPostalCode}
          />
        </div>
        <div className="flex col-2 pt-2 p-0 pr-3">
          <SearchButton
            tooltip="Go"
            onClick={() => {
              console.log('PostalCode', originalPostalCode);

              if (!originalPostalCode || !originalCountry) {
                return;
              }

              setSelectedCountry(originalCountry);
              setSelectedPostalCode(originalPostalCode);

              const newData: SettingDto = { sdPostalCode: originalPostalCode, sdCountry: originalCountry };

              UpdateSetting(newData)
                .then(() => {})
                .catch(() => {});
            }}
          />
        </div>
      </div>
    </div>
  );
};

SchedulesDirectCountrySelector.displayName = 'SchedulesDirectCountrySelector';

export default React.memo(SchedulesDirectCountrySelector);
