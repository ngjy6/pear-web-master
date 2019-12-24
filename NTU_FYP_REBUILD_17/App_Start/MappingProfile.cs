using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using NTU_FYP_REBUILD_17.Dtos;
using NTU_FYP_REBUILD_17.Models;
using NTU_FYP_REBUILD_17.ViewModels;

namespace NTU_FYP_REBUILD_17.App_Start
{
	public class MappingProfile : Profile
	{
		public MappingProfile()
		{
			Mapper.CreateMap<Patient, PatientDto>();
			Mapper.CreateMap<PatientDto, Patient>();

			Mapper.CreateMap<PatientAllocation, PatientAllocationDto>();
			Mapper.CreateMap<PatientAllocationDto, PatientAllocation>();

			Mapper.CreateMap<Patient, PatientAllocationDto>();
			Mapper.CreateMap<PatientAllocationDto, Patient>();

			Mapper.CreateMap<Schedule, SchedulesDto>();
			Mapper.CreateMap<SchedulesDto, Schedule>();

			Mapper.CreateMap<Schedule, zhSchedulesDto>();
			Mapper.CreateMap<zhSchedulesDto, Schedule>();

			Mapper.CreateMap<ApplicationUser, LoginDto>();
			Mapper.CreateMap<LoginDto, ApplicationUser>();

			Mapper.CreateMap<HolidayExperience, HolidayExperienceDto>();
			Mapper.CreateMap<HolidayExperienceDto, HolidayExperience>();

			Mapper.CreateMap<Like, LikesDto>();
			Mapper.CreateMap<LikesDto, Like>();

			Mapper.CreateMap<Dislike, DislikesDto>();
			Mapper.CreateMap<DislikesDto, Dislike>();

			Mapper.CreateMap<Vital, VitalDto>();
			Mapper.CreateMap<VitalDto, Vital>();

			Mapper.CreateMap<Allergy, AllergyDto>();
			Mapper.CreateMap<AllergyDto, Allergy>();

			Mapper.CreateMap<Prescription, PrescriptionDto>();
			Mapper.CreateMap<PrescriptionDto, Prescription>();
			
			Mapper.CreateMap<Routine, RoutineDto>();
			Mapper.CreateMap<RoutineDto, Routine>();

			Mapper.CreateMap<ProblemLog, problemlogDto>();
			Mapper.CreateMap<problemlogDto, ProblemLog>();

			Mapper.CreateMap<ActivityPreference, ActivityPreferenceDto>();
			Mapper.CreateMap<ActivityPreferenceDto, ActivityPreference>();

			Mapper.CreateMap<SocialHistory, SocialHistoryDto>();
			Mapper.CreateMap<SocialHistoryDto, SocialHistory>();

			Mapper.CreateMap<AlbumPatient, AlbumDto>();
			Mapper.CreateMap<AlbumDto, AlbumPatient>();

			Mapper.CreateMap<DoctorNote, DoctorNoteDto>();
			Mapper.CreateMap<DoctorNoteDto, DoctorNote>();

			Mapper.CreateMap<Hobbies, HobbiesDto>();
			Mapper.CreateMap<HobbiesDto, Hobbies>();

			Mapper.CreateMap<Habit, HabitDto>();
			Mapper.CreateMap<HabitDto, Habit>();

			Mapper.CreateMap<PatientAlbumDoctorNoteViewModel, PatientAlbumDoctorNoteDTOViewModel>();
			Mapper.CreateMap<PatientAlbumDoctorNoteDTOViewModel, PatientAlbumDoctorNoteViewModel>();
		}
	}
}