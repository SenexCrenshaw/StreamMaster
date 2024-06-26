import { FieldData } from '@lib/smAPI/smapiTypes';

export const updateFieldInData = (response: any[] | undefined, fieldData: FieldData): any[] | undefined => {
  if (!response) return undefined;

  return response.map((dto: any) => {
    if (dto.Id !== undefined) {
      const id = dto.Id.toString();
      if (id === fieldData.Id) {
        return {
          ...dto,
          [fieldData.Field]: fieldData.Value
        };
      }
    }

    if (dto.Name !== undefined) {
      const name = dto.Name.toString();
      if (name === fieldData.Id) {
        return {
          ...dto,
          [fieldData.Field]: fieldData.Value
        };
      }
    }

    if (dto.ProfileName !== undefined) {
      const name = dto.ProfileName.toString();
      if (name === fieldData.Id) {
        return {
          ...dto,
          [fieldData.Field]: fieldData.Value
        };
      }
    }

    return dto;
  });
};
