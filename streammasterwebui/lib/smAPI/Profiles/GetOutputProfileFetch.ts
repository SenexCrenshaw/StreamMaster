import { GetOutputProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { GetOutputProfileRequest } from '../smapiTypes';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetOutputProfile = createAsyncThunk('cache/getGetOutputProfile', async (param: GetOutputProfileRequest, thunkAPI) => {
  try {
    const response = await GetOutputProfile(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


