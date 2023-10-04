import { useSchedulesDirectGetCountriesQuery } from '@/lib/iptvApi'
import { Dropdown } from 'primereact/dropdown'
import { Toast } from 'primereact/toast'
import React from 'react'

const SchedulesDirectCountrySelector = (
  props: SchedulesDirectCountrySelectorProps,
) => {
  const toast = React.useRef<Toast>(null)
  const [country, setCountry] = React.useState<string>('USA')

  const getCountriesQuery = useSchedulesDirectGetCountriesQuery()

  React.useEffect(() => {
    if (
      props.value !== undefined &&
      props.value !== null &&
      props.value !== ''
    ) {
      setCountry(props.value)
    }
  }, [props.value])

  const options = React.useMemo(() => {
    if (!getCountriesQuery.data) return []

    const countries = []

    if (getCountriesQuery.data['North America']) {
      countries.push(
        ...getCountriesQuery.data['North America']
          .filter(
            (c) => c?.shortName !== undefined && c.shortName.trim() !== '',
          )
          .map((c) => {
            return { label: c.fullName, value: c.shortName }
          }),
      )
    }

    if (getCountriesQuery.data.Europe) {
      countries.push(
        ...getCountriesQuery.data.Europe.filter(
          (c) => c?.shortName !== undefined && c.shortName.trim() !== '',
        ).map((c) => {
          return { label: c.fullName, value: c.shortName }
        }),
      )
    }

    if (getCountriesQuery.data['Latin America']) {
      countries.push(
        ...getCountriesQuery.data['Latin America']
          .filter(
            (c) => c?.shortName !== undefined && c.shortName.trim() !== '',
          )
          .map((c) => {
            return { label: c.fullName, value: c.shortName }
          }),
      )
    }

    if (getCountriesQuery.data.Caribbean) {
      countries.push(
        ...getCountriesQuery.data.Caribbean.filter(
          (c) => c?.shortName !== undefined && c.shortName.trim() !== '',
        ).map((c) => {
          return { label: c.fullName, value: c.shortName }
        }),
      )
    }

    if (getCountriesQuery.data.Oceania) {
      countries.push(
        ...getCountriesQuery.data.Oceania.filter(
          (c) => c?.shortName !== undefined && c.shortName.trim() !== '',
        ).map((c) => {
          return { label: c.shortName + '-' + c.fullName, value: c.shortName }
        }),
      )
    }

    return countries // .sort((a, b) => a.label.localeCompare(b.label));
  }, [getCountriesQuery.data])

  return (
    <>
      <Toast position="bottom-right" ref={toast} />

      <div className="iconSelector flex w-full justify-content-start align-items-center">
        <Dropdown
          className="iconSelector p-0 m-0 w-full"
          filter
          onChange={(e) => {
            setCountry(e.value)
            props.onChange(e.value)
          }}
          options={options}
          placeholder="Country"
          style={{
            ...{
              backgroundColor: 'var(--mask-bg)',
              overflow: 'hidden',
              textOverflow: 'ellipsis',
              whiteSpace: 'nowrap',
            },
          }}
          value={country}
        />
      </div>
    </>
  )
}

SchedulesDirectCountrySelector.displayName = 'SchedulesDirectCountrySelector'

type SchedulesDirectCountrySelectorProps = {
  readonly onChange: (value: string) => void
  readonly value?: string | null
}

export default React.memo(SchedulesDirectCountrySelector)
