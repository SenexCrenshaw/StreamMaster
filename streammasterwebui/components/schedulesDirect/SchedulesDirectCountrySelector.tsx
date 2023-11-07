import SearchButton from '@components/buttons/SearchButton';
import TextInput from '@components/inputs/TextInput';
import { Countries, useSchedulesDirectGetCountriesQuery } from '@lib/iptvApi';
import { useSelectedCountry } from '@lib/redux/slices/selectedCountrySlice';
import { useSelectedZipCode } from '@lib/redux/slices/selectedZipCodeSlice';
import { Dropdown } from 'primereact/dropdown';
import React from 'react';

const SchedulesDirectCountrySelector = (props: SchedulesDirectCountrySelectorProperties) => {
  const { selectedCountry, setSelectedCountry } = useSelectedCountry('Country');
  const { selectedZipCode, setSelectedZipCode } = useSelectedZipCode('ZipCode');

  const getCountriesQuery = useSchedulesDirectGetCountriesQuery();

  React.useEffect(() => {
    if (props.value !== undefined && props.value !== null && props.value !== '') {
      setSelectedCountry(props.value);
    }
  }, [props.value, setSelectedCountry]);

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
    <div className="flex grid pl-1 justify-content-start align-items-center p-0 m-0 w-full">
      <div className="flex col-4">
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
          <TextInput placeHolder="Zip Code" onChange={setSelectedZipCode} value={selectedZipCode} />
        </div>
        <div className="flex col-2 pt-2 p-0 pr-3">
          <SearchButton onClick={(e) => console.log(e)} />
        </div>
      </div>
    </div>
  );
};

SchedulesDirectCountrySelector.displayName = 'SchedulesDirectCountrySelector';

interface SchedulesDirectCountrySelectorProperties {
  readonly onChange: (value: string) => void;
  readonly value?: string | null;
}

export default React.memo(SchedulesDirectCountrySelector);
